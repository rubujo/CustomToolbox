using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using Microsoft.Playwright;
using OpenCCNET;
using Serilog.Events;
using System.IO;
using System.Security.Cryptography;

namespace CustomToolbox.Common.Utils;

/// <summary>
/// Bilibili 短片工具
/// </summary>
public class B23ClipUtil
{
    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="wMain">WMain</param>
    public static void Init(WMain wMain)
    {
        _WMain = wMain;
    }

    /// <summary>
    /// 產生短片檔案
    /// </summary>
    /// <param name="listMID">List&lt;string&gt;，Bilibili 使用者的 mid 值</param>
    /// <param name="folderPath">字串，資料夾路徑</param>
    /// <param name="forceChromium">布林值，是否強制使用 Chromium，預設值為 false</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <param name="openFolderWhenFinished">布林值，是否在處理完成後開啟資料夾，預設值為 true</param>
    /// <param name="exportJsonc">布林值，是否匯出 *.jsonc 格式，預設值為 false</param>
    /// <param name="hansToTW">布林值，是否簡體中文轉換成正體中文，預設值為 true</param>
    public static async Task GenerateClipFiles(
        List<string> listMID,
        string folderPath,
        bool forceChromium = false,
        int maxReTryTimes = 3,
        bool openFolderWhenFinished = true,
        bool exportJsonc = false,
        bool hansToTW = true,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            // 標籤的 ID 值 3（音乐）
            string tagId = "3";

            // 有失敗的 mid。
            List<string> listFailedMID = [];

            // 使用 for 迴圈來批次處理。
            for (int i = 0; i < listMID.Count; i++)
            {
                string mid = listMID[i];

                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgPrepToProduceClipListFile,
                        mid));

                List<ClipData> listClipData = await GetClipList(
                    mid: mid,
                    listFailedMID: ref listFailedMID,
                    tagId: tagId,
                    forceChromium: forceChromium,
                    maxReTryTimes: maxReTryTimes,
                    hansToTW: hansToTW);

                // 短片清單檔案儲存的路徑。
                string filePath = Path.Combine(
                    folderPath,
                    $"{(exportJsonc ? $"{mid}" : $"ClipList_{mid}")}" +
                    $".{(exportJsonc ? "jsonc" : "json")}");

                // 儲存檔案檔案。
                await listClipData.SaveFile(filePath, exportJsonc, ct);

                _WMain?.WriteLog(
                     message: MsgSet.GetFmtStr(
                         MsgSet.MsgClipListFileGeneratedFor,
                         mid,
                         filePath));
            }

            // 判斷是否在處理完成後開啟資料夾。
            if (openFolderWhenFinished)
            {
                // 開啟資料夾。
                CustomFunction.OpenFolder(folderPath);
            }

            // 判斷是否有失敗的 mid。
            if (listFailedMID.Count > 0)
            {
                // TODO: 2024/3/23 代替換成多語系機制。
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgErrorOccured,
                        $"有失敗的 mid：{string.Join(", ", listFailedMID)}"),
                    logEventLevel: LogEventLevel.Error);

                // 針對有失敗的 mid 再重新處理。
                await GenerateClipFiles(
                    listFailedMID,
                    folderPath,
                    forceChromium,
                    maxReTryTimes,
                    openFolderWhenFinished,
                    exportJsonc,
                    hansToTW,
                    ct);
            }
        }
        catch (Exception ex)
        {
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 取得短片列表
    /// </summary>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值，預設值為 3（音乐）</param>
    /// <param name="forceChromium">布林值，是否強制使用 Chromium，預設值為 false</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <param name="hansToTW">布林值，是否簡體中文轉換成正體中文，預設值為 true</param>
    /// <returns>Task&lt;List&lt;ClipData&gt;&gt;</returns>
    private static Task<List<ClipData>> GetClipList(
        string mid,
        ref List<string> listFailedMID,
        string tagId = "3",
        bool forceChromium = false,
        int maxReTryTimes = 3,
        bool hansToTW = true)
    {
        return GetClipListImpl(
            mid,
            listFailedMID,
            tagId,
            forceChromium,
            maxReTryTimes,
            hansToTW);
    }

    /// <summary>
    /// 取得短片列表（Implement）
    /// <para>來源：https://stackoverflow.com/a/20868150</para>
    /// </summary>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值，預設值為 3（音乐）</param>
    /// <param name="forceChromium">布林值，是否強制使用 Chromium，預設值為 false</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <param name="hansToTW">布林值，是否簡體中文轉換成正體中文，預設值為 true</param>
    /// <returns>Task&lt;List&lt;ClipData&gt;&gt;</returns>
    private static async Task<List<ClipData>> GetClipListImpl(
        string mid,
        List<string> listFailedMID,
        string tagId = "3",
        bool forceChromium = false,
        int maxReTryTimes = 3,
        bool hansToTW = true)
    {
        using IPlaywright playwright = await Playwright.CreateAsync();

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(
        new()
        {
            Channel = await PlaywrightUtil.DetectBrowser(forceChromium: forceChromium),
            // 不使用無頭模式，因為會無法通過 Bilibili 網頁的特殊判定以及驗證機制。
            Headless = false
        });

        IPage page = await browser.NewPageAsync();

        string? videos = await DoGetVideosHTML(page, mid, tagId, ref listFailedMID, maxReTryTimes);

        if (string.IsNullOrEmpty(videos))
        {
            // TODO: 2024/3/23 代替換成多語系機制。
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    $"無法取得 mid「{mid}」的影片資訊，取消作業。"),
                logEventLevel: LogEventLevel.Error);

            // 當 mid 不存在於 listFailedMID 時才加入。
            if (!listFailedMID.Any(n => n == mid))
            {
                listFailedMID.Add(mid);
            }

            return [];
        }

        // TODO: 2024/3/23 代替換成多語系機制。
        _WMain?.WriteLog(
            message: MsgSet.GetFmtStr(
                MsgSet.MsgErrorOccured,
                $"第 1 頁 HTML 內容的長度：{videos.Length} bytes"),
            logEventLevel: LogEventLevel.Information);

        List<ClipData> listClipData = [];

        int startIndex = 1;

        List<ClipData> listTempClipData = await GetClipData(
            videos,
            startIndex,
            hansToTW);

        startIndex = listTempClipData
            .OrderByDescending(n => n.No)
            .FirstOrDefault()?.No + 1 ??
            1;

        int pageCount = await GetPageCount(page);

        if (pageCount > 1)
        {
            for (int i = 2; i <= pageCount; i++)
            {
                videos = await DoGetVideosHTMLWithPN(
                    page,
                    mid,
                    tagId,
                    i,
                    ref listFailedMID,
                    maxReTryTimes);

                // TODO: 2024/3/23 代替換成多語系機制。
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgErrorOccured,
                        $"第 {i} 頁 HTML 內容的長度：{videos.Length} bytes"),
                    logEventLevel: LogEventLevel.Information);

                List<ClipData> listRestClipData = await GetClipData(
                    videos,
                    startIndex,
                    hansToTW);

                startIndex = listRestClipData
                    .OrderByDescending(n => n.No)
                    .FirstOrDefault()?.No + 1 ??
                    1;

                listTempClipData.AddRange(listRestClipData);
            }

            // TODO: 2024/3/23 代替換成多語系機制。
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    $"mid「{mid}」作業完成。"),
                logEventLevel: LogEventLevel.Information);
        }
        else
        {
            // TODO: 2024/3/23 代替換成多語系機制。
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    $"無其它分頁，mid「{mid}」作業完成。"),
                logEventLevel: LogEventLevel.Information);
        }

        // 組合 List。
        listClipData.AddRange(listTempClipData);

        // 反向 List。
        listClipData.Reverse();

        // 重新設定 No 的值。
        for (int i = 0; i < listClipData.Count; i++)
        {
            listClipData[i].No = i++;
        }

        return listClipData;
    }

    /// <summary>
    /// 執行取得影片項目的 HTML
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static Task<string> DoGetVideosHTML(
        IPage page,
        string mid,
        string tagId,
        ref List<string> listFailedMID,
        int maxReTryTimes = 3)
    {
        return DoGetVideosHTMLImpl(
            page,
            mid,
            tagId,
            listFailedMID,
            maxReTryTimes);
    }

    /// <summary>
    /// 執行取得影片項目的 HTML（Implement）
    /// <para>來源：https://stackoverflow.com/a/20868150</para>
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static async Task<string> DoGetVideosHTMLImpl(
        IPage page,
        string mid,
        string tagId,
        List<string> listFailedMID,
        int maxReTryTimes = 3)
    {
        int reTryCount = 0;

        string? htmlContent = string.Empty;

        do
        {
            if (reTryCount == maxReTryTimes)
            {
                // TODO: 2024/3/23 代替換成多語系機制。
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgErrorOccured,
                        $"重試已達 {maxReTryTimes} 次，取消作業。"),
                    logEventLevel: LogEventLevel.Warning);

                // 當 mid 不存在於 listFailedMID 時才加入。
                if (!listFailedMID.Any(n => n == mid))
                {
                    listFailedMID.Add(mid);
                }

                break;
            }
            else
            {
                htmlContent = await GetVideosHTML(page, mid, tagId);
            }

            reTryCount++;
        } while (htmlContent.Length <= 0);

        return htmlContent;
    }

    /// <summary>
    /// 執行取得指分頁影片項目的 HTML
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <param name="pageNumber">數值，頁碼</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static Task<string> DoGetVideosHTMLWithPN(
        IPage page,
        string mid,
        string tagId,
        int pageNumer,
        ref List<string> listFailedMID,
        int maxReTryTimes = 3)
    {
        return DoGetVideosHTMLWithPNImpl(
            page,
            mid,
            tagId,
            pageNumer,
            listFailedMID,
            maxReTryTimes);
    }

    /// <summary>
    /// 執行取得指分頁影片項目的 HTML（Implement）
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <param name="pageNumber">數值，頁碼</param>
    /// <param name="listFailedMID">字串列表，失敗的Bilibili 使用者的 mid 值</param>
    /// <param name="maxReTryTimes">數值，最大重試次數，預設值為 3</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static async Task<string> DoGetVideosHTMLWithPNImpl(
        IPage page,
        string mid,
        string tagId,
        int pageNumer,
        List<string> listFailedMID,
        int maxReTryTimes = 3)
    {
        int reTryCount = 0;

        string? htmlContent = string.Empty;

        do
        {
            if (reTryCount == maxReTryTimes)
            {
                // TODO: 2024/3/23 代替換成多語系機制。
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgErrorOccured,
                        $"重試已達 {maxReTryTimes} 次，略過此次的作業。"),
                    logEventLevel: LogEventLevel.Warning);

                // 當 mid 不存在於 listFailedMID 時才加入。
                if (!listFailedMID.Any(n => n == mid))
                {
                    listFailedMID.Add(mid);
                }

                break;
            }
            else
            {
                htmlContent = await GetVideosHtmlWithPN(page, mid, tagId, pageNumer);
            }

            reTryCount++;
        } while (htmlContent.Length <= 0);

        return htmlContent;
    }

    /// <summary>
    /// 取得分頁數
    /// </summary>
    /// <param name="page">IPage</param>
    /// <returns>Task&lt;int&gt;</returns>
    private static async Task<int> GetPageCount(IPage page)
    {
        int pageCount = -1;

        string? totalPagesText = await page.Locator(".be-pager-total").TextContentAsync();

        if (!string.IsNullOrEmpty(totalPagesText))
        {
            totalPagesText = totalPagesText.Trim()
                .Replace("共", string.Empty)
                .Replace("页", string.Empty)
                .Replace("，", string.Empty);

            pageCount = int.TryParse(totalPagesText, out int totalPages) ? totalPages : -1;
        }

        return pageCount;
    }

    /// <summary>
    /// 取得影片項目的 HTML
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static async Task<string> GetVideosHTML(IPage page, string mid, string tagId)
    {
        // 必須先執行。
        await SimulateRandomMouseMovement(page);

        await page.GotoAsync($"https://space.bilibili.com/{mid}/video?tid={tagId}&keyword=&order=pubdate");

        // 增加隨機數。
        await SimulateRandomMouseMovement(page);

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        string htmlContent = await page.Locator("#submit-video-list > ul.list-list").InnerHTMLAsync()
            ?? string.Empty;

        return htmlContent;
    }

    /// <summary>
    /// 取得指分頁影片項目的 HTML
    /// </summary>
    /// <param name="page">IPage</param>
    /// <param name="mid">字串，Bilibili 使用者的 mid 值</param>
    /// <param name="tagId">字串，標籤的 ID 值</param>
    /// <param name="pageNumber">數值，頁碼</param>
    /// <returns>Task&lt;string&gt;</returns>
    private static async Task<string> GetVideosHtmlWithPN(IPage page, string mid, string tagId, int pageNumber)
    {
        // 必須先執行。
        await SimulateRandomMouseMovement(page);

        await page.GotoAsync($"https://space.bilibili.com/{mid}/video?tid={tagId}&pn={pageNumber}&keyword=&order=pubdate");

        // 增加隨機數。
        await SimulateRandomMouseMovement(page);

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        string htmlContent = await page.Locator("#submit-video-list > ul.list-list").InnerHTMLAsync()
            ?? string.Empty;

        return htmlContent;
    }

    /// <summary>
    /// 模擬滑鼠隨機移動
    /// </summary>
    /// <param name="page">IPage</param>
    private static async Task SimulateRandomMouseMovement(IPage page)
    {
        int loopCount = RandomNumberGenerator.GetInt32(50, 100),
            x,
            y;

        for (int i = 0; i < loopCount; i++)
        {
            x = RandomNumberGenerator.GetInt32(100, 200);
            y = RandomNumberGenerator.GetInt32(100, 200);

            if (i % 2 == 0)
            {
                // 移動滑鼠。
                await page.Mouse.MoveAsync(x, y);

                // 滾動滑鼠滾輪。
                await page.Mouse.WheelAsync(y, x);
            }
            else
            {
                // 移動滑鼠。
                await page.Mouse.MoveAsync(y, x);

                // 滾動滑鼠滾輪。
                await page.Mouse.WheelAsync(x, y);
            }
        }
    }

    /// <summary>
    /// 取得短片資料
    /// </summary>
    /// <param name="htmlContent">字串，HTML 內容</param>
    /// <param name="startIndex">數值，起始的索引值，預設值為 1</param>
    /// <param name="hansToTW">布林值，是否簡體中文轉換成正體中文，預設值為 true</param>
    /// <returns>Task&lt;List&lt;ClipData&gt;&gt;</returns>
    private static async Task<List<ClipData>> GetClipData(
        string htmlContent,
        int startIndex = 1,
        bool hansToTW = true)
    {
        IConfiguration configuration = Configuration.Default;

        IBrowsingContext browsingContext = BrowsingContext.New(configuration);

        IDocument document = await browsingContext.OpenAsync(request => request.Content(htmlContent));

        if (document.Body == null)
        {
            return [];
        }

        List<ClipData> listClipData = [];

        foreach (INode node in document.Body.ChildNodes)
        {
            ClipData clipData = new()
            {
                No = startIndex,
                StartTime = TimeSpan.Zero,
                SubtitleFileUrl = null,
                IsAudioOnly = false,
                IsLivestream = false
            };

            // 取得影片的標題。
            INode? nodeDIV = node.ChildNodes.FirstOrDefault(n => n.NodeName == "DIV");
            INode? nodeDIVTitleRow = nodeDIV?.ChildNodes.FirstOrDefault(n => n.NodeName == "DIV" && ((IHtmlElement)n).ClassName == "title-row");
            INode? nodeATitle = nodeDIVTitleRow?.ChildNodes.FirstOrDefault(n => n.NodeName == "A");
            // 取得影片的網址。
            INode? nodeAUrl = node.ChildNodes.FirstOrDefault(n => n.NodeName == "A");
            // 取得影片的長度。
            INode? nodeSpanLength = nodeAUrl?.ChildNodes.FirstOrDefault(n => n.NodeName == "SPAN" && ((IHtmlElement)n).ClassName == "length");

            if (nodeATitle is IHtmlAnchorElement haeTitle)
            {
                string title = haeTitle.TextContent;

                clipData.Name = hansToTW ? ZhConverter.HansToTW(title, isIdiomConvert: true) : title;
            }

            // 排除影片標題帶有特殊字詞的影片。
            if (!CheckVideoTitle(clipData.Name))
            {
                continue;
            }

            if (nodeAUrl is IHtmlAnchorElement haeUrl)
            {
                string videoUrl = haeUrl.Href;

                // 判斷解析出來的網址是否是 "http://" 開頭。
                if (videoUrl.StartsWith("http://"))
                {
                    // 將 "http://" 替換成 "https://"。
                    videoUrl = videoUrl.Replace("http://", "https://");
                }

                // 當網址的結尾是 "/" 時。
                if (videoUrl.EndsWith('/'))
                {
                    // 移除最後一個字元。
                    videoUrl = videoUrl[..^1];
                }

                clipData.VideoUrlOrID = videoUrl;
            }

            if (nodeSpanLength is IHtmlSpanElement haeSpanLength)
            {
                string length = haeSpanLength.TextContent;

                // 判斷字串的長度，來決定是否要補前綴。
                if (length.Length == 5)
                {
                    length = $"00:{length}";
                }

                clipData.EndTime = TimeSpan.TryParse(length, out TimeSpan result) ? result : TimeSpan.Zero;
            }

            if (!string.IsNullOrEmpty(clipData.VideoUrlOrID))
            {
                listClipData.Add(clipData);
            }

            startIndex++;
        }

        return listClipData;
    }

    /// <summary>
    /// 檢查標題
    /// </summary>
    /// <param name="value">字串，影片的標題</param>
    /// <returns>布林值</returns>
    private static bool CheckVideoTitle(string? value)
    {
        bool isOkay = true;

        string[] excludedPhrases = Properties.Settings.Default
            .B23ClipListExcludedPhrases
            .Split(
                ";".ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);

        foreach (string phrase in excludedPhrases)
        {
            if (value?.Contains(phrase) == true)
            {
                isOkay = false;

                break;
            }
        }

        return isOkay;
    }
}