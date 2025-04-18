using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Extensions;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Microsoft.Playwright;
using Microsoft.Win32;
using Serilog.Events;

namespace CustomToolbox.Common.Utils;

public class PlaywrightUtil
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
    /// 設定 HTML 碼
    /// </summary>
    /// <param name="value">字串，值</param>
    /// <returns>字串</returns>
    public static string SetHTML(char value)
    {
        string output;

        if (value == ',')
        {
            output = "<span class=\\\"odometer-formatting-mark\\\">,</span>";
        }
        else
        {
            output = "<span class=\\\"odometer-digit\\\">" +
                "<span class=\\\"odometer-digit-spacer\\\">8</span>" +
                "<span class=\\\"odometer-digit-inner\\\">" +
                "<span class=\\\"odometer-ribbon\\\">" +
                "<span class=\\\"odometer-ribbon-inner\\\"" +
                $"<span class=\\\"odometer-channelName\\\">{value}</span>" +
                "</span>" +
                "</span>" +
                "</span>" +
                "</span>";
        }

        return output;
    }

    /// <summary>
    /// 取得 Clip
    /// </summary>
    /// <param name="subscriberAmount">數值，訂閱者的數量</param>
    /// <returns>Clip</returns>
    public static Clip? GetClip(decimal subscriberAmount)
    {
        Clip? clip;

        if (subscriberAmount == -1)
        {
            clip = null;
        }
        else if (subscriberAmount <= 9999999)
        {
            clip = new()
            {
                X = 390,
                Y = 135,
                Width = 500,
                Height = 500
            };
        }
        else if (subscriberAmount <= 99999999)
        {
            clip = new()
            {
                X = 340,
                Y = 115,
                Width = 600,
                Height = 600
            };
        }
        else if (subscriberAmount <= 999999999)
        {
            clip = new()
            {
                X = 290,
                Y = 15,
                Width = 700,
                Height = 700
            };
        }
        else
        {
            clip = null;

            _WMain?.WriteLog(
                message: MsgSet.MsgYtscToolSubscribersTooLongCancelClip,
                logEventLevel: LogEventLevel.Warning);
        }

        return clip;
    }

    /// <summary>
    /// 取得 int[]
    /// </summary>
    /// <param name="channelName">字串，頻道名稱</param>
    /// <returns>數值陣列</returns>
    public static int[] GetIntArray(string channelName)
    {
        int step = channelName.Length / VariableSet.SplitLength;
        int leftNum = channelName.Length % VariableSet.SplitLength;

        if (leftNum != 0)
        {
            step++;
        }

        int[] output = new int[step];

        for (int i = 0; i < step; i++)
        {
            if (leftNum == 0)
            {
                output[i] = VariableSet.SplitLength;
            }
            else
            {
                if (i != step - 1)
                {
                    output[i] = VariableSet.SplitLength;
                }
                else
                {
                    output[i] = leftNum;
                }
            }
        }

        return output;
    }

    /// <summary>
    /// 偵測網頁瀏覽器
    /// </summary>
    /// <param name="forceChromium">布林值，是否強制使用 Chromium，預設值為 false</param>
    /// <returns>Task&lt;string&gt;</returns>
    public static async Task<string> DetectBrowser(bool forceChromium = false)
    {
        string browser = "chromuim";

        if (IsBrowserInstalled("chrome"))
        {
            browser = "chrome";
        }
        else if (IsBrowserInstalled("edge"))
        {
            browser = "msedge";
        }
        else
        {
            await InstallPlaywrightBrowser();
        }

        if (forceChromium)
        {
            browser = "chromium";

            await InstallPlaywrightBrowser();
        }

        return browser;
    }

    /// <summary>
    /// 安裝 Playwright 的網頁瀏覽器
    /// </summary>
    /// <returns>Task</returns>
    public static async Task InstallPlaywrightBrowser()
    {
        await Task.Run(() =>
        {
            try
            {
                string[] args = ["install", "chromium"];

                // 安裝 Playwright 的瀏覽器。
                int exitCode = Program.Main(args);

                if (exitCode != 0)
                {
                    throw new Exception(MsgSet.GetFmtStr(
                        MsgSet.MsgPlaywrightException,
                        exitCode.ToString()));
                }

                _WMain?.WriteLog(message: MsgSet.MsgPlaywrightBrowserPath);
                _WMain?.WriteLog(message: $@"C:\Users\{Environment.UserName}\AppData\Local\ms-playwright");
            }
            catch (Exception ex)
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgErrorOccured,
                        ex.GetExceptionMessage()),
                    logEventLevel: LogEventLevel.Error);
            }
        });
    }

    /// <summary>
    /// 檢查網頁瀏覽器是否已安裝
    /// <para>來源： https://quomodo-info.com/csharp-check-if-webbrowser-is-installed/</para>
    /// </summary>
    /// <param name="browserName">字串，網頁瀏覽器的名稱</param>
    /// <returns>布林值</returns>
    public static bool IsBrowserInstalled(string browserName)
    {
        bool isInstalled = false;

        RegistryKey? browsersNode = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

        if (browsersNode != null)
        {
            foreach (string browser in browsersNode.GetSubKeyNames())
            {
                if (browser.Contains(browserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    isInstalled = true;

                    break;
                }
            }
        }

        return isInstalled;
    }

    /// <summary>
    /// 取得 YouTube 頻道的 ID
    /// </summary>
    /// <param name="url">字串，YouTube 頻道的網址</param>
    /// <returns>字串</returns>
    public static string GetYTChannelID(string url)
    {
        string channelID = string.Empty;

        HtmlWeb htmlWeb = new();
        HtmlDocument htmlDocument = htmlWeb.Load(url);

        HtmlNodeCollection? metaTags = htmlDocument.DocumentNode.SelectNodes("//meta");

        if (metaTags != null)
        {
            HtmlNode? ogUrl = metaTags.FirstOrDefault(n => n.Attributes["property"] != null &&
                n.Attributes["content"] != null &&
                n.Attributes["property"].Value == "og:url");

            if (ogUrl != null)
            {
                channelID = ogUrl.Attributes["content"].Value;
            }
        }

        return channelID;
    }
}