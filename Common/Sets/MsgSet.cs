using Application = System.Windows.Application;

namespace CustomToolbox.Common.Sets;

/// <summary>
/// 訊息組
/// </summary>
public class MsgSet
{
    #region 應用程式

    /// <summary>
    /// 應用程式：應用程式名稱
    /// </summary>
    public static readonly string AppName = Application.Current
        .FindResource("AppName").ToString() ??
        string.Empty;

    /// <summary>
    /// 應用程式：應用程式關於
    /// </summary>
    public static readonly string AppAbout = Application.Current
        .FindResource("AppAbout").ToString() ??
        string.Empty;

    /// <summary>
    /// 應用程式：應用程式簡述
    /// </summary>
    public static readonly string AppDescription = Application.Current
        .FindResource("AppDescription").ToString() ??
        string.Empty;

    #endregion

    #region 字詞

    /// <summary>
    /// 字詞：省略號
    /// </summary>
    public static readonly string Ellipses = Application.Current
        .FindResource("Ellipses").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：關於
    /// </summary>
    public static readonly string About = Application.Current
        .FindResource("About").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：（無）
    /// </summary>
    public static readonly string ClipTitle = Application.Current
        .FindResource("ClipTitle").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：短片名稱
    /// </summary>
    public static readonly string ClipTitleToolTip = Application.Current
        .FindResource("ClipTitleToolTip").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：隱藏
    /// </summary>
    public static readonly string Hide = Application.Current
        .FindResource("Hide").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：顯示
    /// </summary>
    public static readonly string Show = Application.Current
        .FindResource("Show").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：靜音
    /// </summary>
    public static readonly string Mute = Application.Current
        .FindResource("Mute").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：取消靜音
    /// </summary>
    public static readonly string Unmute = Application.Current
        .FindResource("Unmute").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：啟用
    /// </summary>
    public static readonly string Enable = Application.Current
        .FindResource("Enable").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：停用
    /// </summary>
    public static readonly string Disable = Application.Current
        .FindResource("Disable").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：啟用
    /// </summary>
    public static readonly string Enabled = Application.Current
        .FindResource("Enabled").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：停用
    /// </summary>
    public static readonly string Disabled = Application.Current
        .FindResource("Disabled").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：暫停
    /// </summary>
    public static readonly string Pause = Application.Current
        .FindResource("Pause").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：恢復播放
    /// </summary>
    public static readonly string Resume = Application.Current
        .FindResource("Resume").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：請選擇
    /// </summary>
    public static readonly string SelectPlease = Application.Current
        .FindResource("SelectPlease").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：正在播放
    /// </summary>
    public static readonly string StatePlaying = Application.Current
        .FindResource("StatePlaying").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：暫停播放
    /// </summary>
    public static readonly string StatePause = Application.Current
        .FindResource("StatePause").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：停止播放
    /// </summary>
    public static readonly string StateStop = Application.Current
        .FindResource("StateStop").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：頻道：
    /// </summary>
    public static readonly string VideoDataChannel = Application.Current
        .FindResource("VideoDataChannel").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：上傳者：
    /// </summary>
    public static readonly string VideoDataUploader = Application.Current
        .FindResource("VideoDataUploader").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：上傳時間：
    /// </summary>
    public static readonly string VideoDataUploadDate = Application.Current
        .FindResource("VideoDataUploadDate").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：影片 ID 值：
    /// </summary>
    public static readonly string VideoDataID = Application.Current
        .FindResource("VideoDataID").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：影片標題：
    /// </summary>
    public static readonly string VideoDataTitle = Application.Current
        .FindResource("VideoDataTitle").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：觀看次數：
    /// </summary>
    public static readonly string VideoDataViewCount = Application.Current
        .FindResource("VideoDataViewCount").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：描述：
    /// </summary>
    public static readonly string VideoDataDescription = Application.Current
        .FindResource("VideoDataDescription").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：格式：
    /// </summary>
    public static readonly string VideoDataFormats = Application.Current
        .FindResource("VideoDataFormats").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：目前已有
    /// </summary>
    public static readonly string YtscToolPrefixText = Application.Current
        .FindResource("YtscToolPrefixText").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：位 YouTube 訂閱者
    /// </summary>
    public static readonly string YtscToolSuffixText = Application.Current
        .FindResource("YtscToolSuffixText").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：影片索引值：{0}
    /// </summary>
    public static readonly string YtdlSharpVideoIndex = Application.Current
        .FindResource("YtdlSharpVideoIndex").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：下載速度：{0}
    /// </summary>
    public static readonly string YtdlSharpDownloadSpeed = Application.Current
        .FindResource("YtdlSharpDownloadSpeed").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：剩餘時間：{0}
    /// </summary>
    public static readonly string YtdlSharpETA = Application.Current
        .FindResource("YtdlSharpETA").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：檔案大小：{0}
    /// </summary>
    public static readonly string YtdlSharpTotalDownloadSize = Application.Current
        .FindResource("YtdlSharpTotalDownloadSize").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：資料：{0}
    /// </summary>
    public static readonly string YtdlSharpData = Application.Current
        .FindResource("YtdlSharpData").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：是
    /// </summary>
    public static readonly string Yes = Application.Current
        .FindResource("Yes").ToString() ??
        string.Empty;

    /// <summary>
    /// 字詞：否
    /// </summary>
    public static readonly string No = Application.Current
        .FindResource("No").ToString() ??
        string.Empty;

    #endregion

    #region 字串模板

    /// <summary>
    /// 字串模板：[維護者：{0}]
    /// </summary>
    public static readonly string TemplateMaintainer = Application.Current
        .FindResource("TemplateMaintainer").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[{0}][{1}（{2}）]：（{3}）{4}
    /// </summary>
    public static readonly string TemplateDiscordRichPresenceOnPresenceUpdate = Application.Current
        .FindResource("TemplateDiscordRichPresenceOnPresenceUpdate").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[{0}][{1}]：{2}
    /// </summary>
    public static readonly string TemplateDiscordRichPresenceOnError = Application.Current
        .FindResource("TemplateDiscordRichPresenceOnError").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[{0}][{1}]：{2}
    /// </summary>
    public static readonly string TemplateDiscordRichPresenceOnClose = Application.Current
        .FindResource("TemplateDiscordRichPresenceOnClose").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[Xabe.FFmpeg][資料接收]：{0}
    /// </summary>
    public static readonly string TemplateXabeFFmpegOnDataReceived = Application.Current
        .FindResource("TemplateXabeFFmpegOnDataReceived").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[Xabe.FFmpeg][進度]：處理識別：{0} [{1}/{2}] {3}%
    /// </summary>
    public static readonly string TemplateXabeFFmpegOnProgress = Application.Current
        .FindResource("TemplateXabeFFmpegOnProgress").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：[Xabe.FFmpeg] 作業完成。&#x0a;開始時間：{0}&#x0a;結束時間：{1}&#x0a;耗時：{2}&#x0a;使用的參數：{3}
    /// </summary>
    public static readonly string TemplateXabeFFmpegConversionResult = Application.Current
        .FindResource("TemplateXabeFFmpegConversionResult").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：已更新短片「{0}」的開始時間至「{1}」。
    /// </summary>
    public static readonly string TemplateUpdateStarTimeOfClipTo = Application.Current
        .FindResource("TemplateUpdateStarTimeOfClipTo").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：已更新短片「{0}」的結束時間至「{1}」。
    /// </summary>
    public static readonly string TemplateUpdateEndTimeOfClipTo = Application.Current
        .FindResource("TemplateUpdateEndTimeOfClipTo").ToString() ??
        string.Empty;

    /// <summary>
    /// 字串模板：{0}（{1}）
    /// </summary>
    public static readonly string TemplateWhipserDetectLaunguageResult = Application.Current
        .FindResource("TemplateWhipserDetectLaunguageResult").ToString() ??
        string.Empty;

    #endregion

    #region 控制項

    /// <summary>
    /// 控制項：啟用不顯示影像
    /// </summary>
    public static readonly string MIEnableNoVideo = Application.Current
        .FindResource("MIEnableNoVideo").ToString() ??
        string.Empty;

    /// <summary>
    /// 控制項：停用不顯示影像
    /// </summary>
    public static readonly string MIDisableNoVideo = Application.Current
        .FindResource("MIDisableNoVideo").ToString() ??
        string.Empty;

    #endregion

    #region 對話視窗

    /// <summary>
    /// 對話視窗：選擇檔案
    /// </summary>
    public static readonly string LoadClipListSelectFile = Application.Current
        .FindResource("LoadClipListSelectFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：JSON 檔案（*.json）|*.json|JSON 檔案（含備註）|*.jsonc|時間標記文字檔案（*.txt）|*.txt
    /// </summary>
    public static readonly string LoadClipListFilter = Application.Current
        .FindResource("LoadClipListFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：儲存檔案
    /// </summary>
    public static readonly string SaveClipListSelectFile = Application.Current
        .FindResource("SaveClipListSelectFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：短片清單檔案（*.json）|*.json|時間標記播放清單檔案（*.json）|*.json|秒數播放清單檔案（*.json）|*.json|JSON 檔案（含備註）|*.jsonc|時間標記文字檔案（*.txt）|*.txt
    /// </summary>
    public static readonly string SaveClipListFilter = Application.Current
        .FindResource("SaveClipListFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：選擇路徑
    /// </summary>
    public static readonly string SelectProfilePath = Application.Current
        .FindResource("SelectProfilePath").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：文字檔（*.txt）|.txt
    /// </summary>
    public static readonly string ExportLogFilter = Application.Current
        .FindResource("ExportLogFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：選擇視訊檔案
    /// </summary>
    public static readonly string SelectVideoFile = Application.Current
        .FindResource("SelectVideoFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：MPEG-4 Part 14|*.mp4|Matroska 視訊檔|*.mkv
    /// </summary>
    public static readonly string SelectVideoFileFilter = Application.Current
        .FindResource("SelectVideoFileFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：選擇檔案
    /// </summary>
    public static readonly string SelectWhisperInputFile = Application.Current
        .FindResource("SelectWhisperInputFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：MPEG-4 Part 14|*.mp4|Matroska 視訊檔|*.mkv|Matroska 音訊檔|*.mka|MPEG-4 Part 3|*.m4a|AAC|*.aac|MPEG-1 Layer 3|*.mp3|WAVE|*.wav|Ogg Vorbis|*.ogg|FLAC|*.flac|Opus|*.opus
    /// </summary>
    public static readonly string SelectWhisperInputFileFilter = Application.Current
        .FindResource("SelectWhisperInputFileFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：選擇字幕檔案
    /// </summary>
    public static readonly string SelectSubtitleFile = Application.Current
        .FindResource("SelectSubtitleFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：SupRip Text|*.srt|WebVTT|*.vtt|SubStation Alpha|*.ssa|Advanced SubStation Alpha|*.ass
    /// </summary>
    public static readonly string SelectSubtitleFileFilter = Application.Current
        .FindResource("SelectSubtitleFileFilter").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：儲存檔案
    /// </summary>
    public static readonly string SaveImageFile = Application.Current
        .FindResource("SaveImageFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 對話視窗：PNG 影像檔 (*.png)|*.png|JPEG 影像檔 (*.jpg)|*.jpg
    /// </summary>
    public static readonly string SaveImageFileFilter = Application.Current
        .FindResource("SaveImageFileFilter").ToString() ??
        string.Empty;

    #endregion

    #region 訊息

    /// <summary>
    /// 訊息：開始下載 {0}。
    /// </summary>
    public static readonly string MsgStartDownloading = Application.Current
        .FindResource("MsgStartDownloading").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已下載 {0}。
    /// </summary>
    public static readonly string? MsgDownloaded = Application.Current
        .FindResource("MsgDownloaded").ToString();

    /// <summary>
    /// 訊息：發生錯誤 {0}。
    /// </summary>
    public static readonly string MsgErrorOccured = Application.Current
        .FindResource("MsgErrorOccured").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已解壓縮 {0}。
    /// </summary>
    public static readonly string MsgDecompressed = Application.Current
        .FindResource("MsgDecompressed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已刪除 {0}。
    /// </summary>
    public static readonly string MsgDeleted = Application.Current
        .FindResource("MsgDeleted").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已建立 {0}。
    /// </summary>
    public static readonly string MsgCreated = Application.Current
        .FindResource("MsgCreated").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已編輯 {0}。
    /// </summary>
    public static readonly string MsgEdited = Application.Current
        .FindResource("MsgEdited").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新 {0}。
    /// </summary>
    public static readonly string MsgUpdated = Application.Current
        .FindResource("MsgUpdated").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已找到 {0}。
    /// </summary>
    public static readonly string MsgFound = Application.Current
        .FindResource("MsgFound").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：雙擊以開關彈出視窗
    /// </summary>
    public static readonly string MsgDoubleClickToTogglePopupWindow = Application.Current
        .FindResource("MsgDoubleClickToTogglePopupWindow").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消下載檔案。
    /// </summary>
    public static readonly string MsgDownloadCanceled = Application.Current
        .FindResource("MsgDownloadCanceled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消下載 {0}。
    /// </summary>
    public static readonly string MsgCancelFileDownload = Application.Current
        .FindResource("MsgCancelFileDownload").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：下載時發生例外，錯誤訊息 {0}
    /// </summary>
    public static readonly string DownloadError = Application.Current
        .FindResource("MsgDownloadError").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：檔案下載完成。
    /// </summary>
    public static readonly string MsgDownloadFinished = Application.Current
        .FindResource("MsgDownloadFinished").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您確定要強制重新下載相依性檔案嗎？
    /// </summary>
    public static readonly string MsgReDownloadDeps = Application.Current
        .FindResource("MsgReDownloadDeps").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：無法儲存短片清單檔案，短片清單沒有任何資料。
    /// </summary>
    public static readonly string MsgCanNotSaveClipList = Application.Current
        .FindResource("MsgCanNotSaveClipList").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請選擇有效的短片清單檔案。
    /// </summary>
    public static readonly string MsgSelectAValidClipListFile = Application.Current
        .FindResource("MsgSelectAValidClipListFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：不支援的網站。
    /// </summary>
    public static readonly string MsgUnSupportedSite = Application.Current
        .FindResource("MsgUnSupportedSite").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請先選擇要播放的短片。
    /// </summary>
    public static readonly string MsgSelectTheClipToPlay = Application.Current
        .FindResource("MsgSelectTheClipToPlay").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：短片資料獲取成功。
    /// </summary>
    public static readonly string MsgClipInfoFetchSuceed = Application.Current
        .FindResource("MsgClipInfoFetchSuceed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：短片資料獲取失敗。
    /// </summary>
    public static readonly string MsgClipInfoFetchFailed = Application.Current
        .FindResource("MsgClipInfoFetchFailed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請先選擇要獲取資訊的短片。
    /// </summary>
    public static readonly string MsgSelectTheClipToFetchInfo = Application.Current
        .FindResource("MsgSelectTheClipToFetchInfo").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請先選擇要下載的短片。
    /// </summary>
    public static readonly string MsgSelectTheClipToDownload = Application.Current
        .FindResource("MsgSelectTheClipToDownload").ToString() ??
        string.Empty;

    /// <summary>
    /// 已清除日誌紀錄。
    /// </summary>
    public static readonly string MsgLogCleared = Application.Current
        .FindResource("MsgLogCleared").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已匯出日誌紀錄至 {0}。
    /// </summary>
    public static readonly string MsgExportLogTo = Application.Current
        .FindResource("MsgExportLogTo").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已於 {0} 檢查 yt-dlp 是否有新版本，故今日不會再自動檢查；如有需要，請手動執行更新。
    /// </summary>
    public static readonly string MsgYtDlpNotice = Application.Current
        .FindResource("MsgYtDlpNotice").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請手動點選 yt-dlp/FFmpeg-Builds 按鈕以更新 FFmpeg。
    /// </summary>
    public static readonly string MsgUpdateFFmpeg = Application.Current
        .FindResource("MsgUpdateFFmpeg").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：10 秒後自動重新啟動應用程式。
    /// </summary>
    public static readonly string MsgAutoRestartApplicationAfterTenSeconds = Application.Current
        .FindResource("MsgAutoRestartApplicationAfterTenSeconds").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請重新啟動應用程式。
    /// </summary>
    public static readonly string MsgRestartApplication = Application.Current
        .FindResource("MsgRestartApplication").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：播放失敗，無效的短片 ID 或是網址。
    /// </summary>
    public static readonly string MsgInvalidVideoIDOrUrl = Application.Current
        .FindResource("MsgInvalidVideoIDOrUrl").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已暫停播放短片。
    /// </summary>
    public static readonly string MsgClipPaused = Application.Current
        .FindResource("MsgClipPaused").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已恢復播放短片。
    /// </summary>
    public static readonly string MsgClipResumed = Application.Current
        .FindResource("MsgClipResumed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已靜音短片。
    /// </summary>
    public static readonly string MsgClipMuted = Application.Current
        .FindResource("MsgClipMuted").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消靜音短片。
    /// </summary>
    public static readonly string MsgClipUnmuted = Application.Current
        .FindResource("MsgClipUnmuted").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：正在播放短片。
    /// </summary>
    public static readonly string MsgPlayingClip = Application.Current
        .FindResource("MsgPlayingClip").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：正在播放：{0}
    /// </summary>
    public static readonly string MsgNowPlaying = Application.Current
        .FindResource("MsgNowPlaying").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：隨機播放失敗，短片清單無資料。
    /// </summary>
    public static readonly string MsgRandomPlayFailedClipListNoData = Application.Current
        .FindResource("MsgRandomPlayFailedClipListNoData").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已載入多媒體。
    /// </summary>
    public static readonly string MsgMediaLoaded = Application.Current
        .FindResource("MsgMediaLoaded").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已結束播放多媒體。
    /// </summary>
    public static readonly string MsgMediaFinished = Application.Current
        .FindResource("MsgMediaFinished").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已暫停播放多媒體。
    /// </summary>
    public static readonly string MsgMediaPaused = Application.Current
        .FindResource("MsgMediaPaused").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已恢復播放多媒體。
    /// </summary>
    public static readonly string MsgMediaResumed = Application.Current
        .FindResource("MsgMediaResumed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已開始緩衝多媒體……
    /// </summary>
    public static readonly string MsgMediaStartedBuffering = Application.Current
        .FindResource("MsgMediaStartedBuffering").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已結束緩衝多媒體。
    /// </summary>
    public static readonly string MsgMediaEndedBuffering = Application.Current
        .FindResource("MsgMediaEndedBuffering").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已開始搜尋多媒體……
    /// </summary>
    public static readonly string MsgMediaStartedSeeking = Application.Current
        .FindResource("MsgMediaStartedSeeking").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已結束搜尋多媒體。
    /// </summary>
    public static readonly string MsgMediaEndedSeeking = Application.Current
        .FindResource("MsgMediaEndedSeeking").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤，多媒體播放失敗。
    /// </summary>
    public static readonly string MsgMediaError = Application.Current
        .FindResource("MsgMediaError").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已卸載多媒體。
    /// </summary>
    public static readonly string MsgMediaUnloaded = Application.Current
        .FindResource("MsgMediaUnloaded").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已啟用不顯示影像。
    /// </summary>
    public static readonly string MsgNoVideoEnabled = Application.Current
        .FindResource("MsgNoVideoEnabled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已停用不顯示影像。
    /// </summary>
    public static readonly string MsgNoVideoDisabled = Application.Current
        .FindResource("MsgNoVideoDisabled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已設定 YouTube 影片畫質，但在播放下一個短片時才會生效。
    /// </summary>
    public static readonly string MsgYouTubeVideoQualityChanged = Application.Current
        .FindResource("MsgYouTubeVideoQualityChanged").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：播放失敗，短片清單無資料。
    /// </summary>
    public static readonly string MsgPlayFailedClipListNoData = Application.Current
        .FindResource("MsgPlayFailedClipListNoData").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：無上一個短片，短片清單已播畢。
    /// </summary>
    public static readonly string MsgNoPreviousClip = Application.Current
        .FindResource("MsgNoPreviousClip").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：無下一個短片，短片清單已播畢。
    /// </summary>
    public static readonly string MsgNoNextClip = Application.Current
        .FindResource("MsgNoNextClip").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已停止播放短片。
    /// </summary>
    public static readonly string MsgStoppedPlayingClip = Application.Current
        .FindResource("MsgStoppedPlayingClip").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您確定要將應用程式切換使用此（{0}）語系嗎？變更語系會需要重新啟動應用程式，請確認您目前的作業皆已完成。如要繼續，請按「確定」按鈕。
    /// </summary>
    public static readonly string MsgChangeLanguage = Application.Current
        .FindResource("MsgChangeLanguage").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您一次只能選擇一個短片清單。
    /// </summary>
    public static readonly string MsgSelectAClipListAtSameTime = Application.Current
        .FindResource("MsgSelectAClipListAtSameTime").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請選擇有效的短片清單。
    /// </summary>
    public static readonly string MsgSelectAValidClipList = Application.Current
        .FindResource("MsgSelectAValidClipList").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新不支援的域名。
    /// </summary>
    public static readonly string MsgUpdateUnsupportedDomains = Application.Current
        .FindResource("MsgUpdateUnsupportedDomains").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新附加秒數。
    /// </summary>
    public static readonly string MsgUpdateAppendSeconds = Application.Current
        .FindResource("MsgUpdateAppendSeconds").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請輸入有效的數值。
    /// </summary>
    public static readonly string MsgInputAValidNumber = Application.Current
        .FindResource("MsgInputAValidNumber").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：目標資料夾不存在。
    /// </summary>
    public static readonly string MsgPathIsNotExists = Application.Current
        .FindResource("MsgPathIsNotExists").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新使用者代理字串。
    /// </summary>
    public static readonly string MsgUpdateUserAgent = Application.Current
        .FindResource("MsgUpdateUserAgent").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您想將目前的短片清單，儲存至短片清單暫存檔案，並結束本應用程式嗎？
    /// </summary>
    public static readonly string MsgConfirmExitApp = Application.Current
        .FindResource("MsgConfirmExitApp").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您選擇的視訊檔案為不支援的格式。
    /// </summary>
    public static readonly string MsgSelectedVideoNonSupported = Application.Current
        .FindResource("MsgSelectedVideoNonSupported").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請輸入 Bilibili 使用者 mid。
    /// </summary>
    public static readonly string MsgB23UserMidCantBeEmpty = Application.Current
        .FindResource("MsgB23UserMidCantBeEmpty").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新排除字詞。
    /// </summary>
    public static readonly string MsgUpdateLB23ClipListExcludedPhrases = Application.Current
        .FindResource("MsgUpdateLB23ClipListExcludedPhrases").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：偵測到排除字詞，已忽略此影片：{0}
    /// </summary>
    public static readonly string MsgSkipThisVideo = Application.Current
        .FindResource("MsgSkipThisVideo").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：偵測影片網址（{0}）無效，已忽略此影片：{1}
    /// </summary>
    public static readonly string MsgInvalidUrlSkipThisVideo = Application.Current
        .FindResource("MsgInvalidUrlSkipThisVideo").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請輸入 YouTube 頻道 ID。
    /// </summary>
    public static readonly string MsgYtChannelIDCCantNoBeEmpty = Application.Current
        .FindResource("MsgYtChannelIDCCantNoBeEmpty").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：Playwright 使用的網頁瀏覽器頻道：{0}
    /// </summary>
    public static readonly string MsgYtscToolUsedBrowserChannel = Application.Current
        .FindResource("MsgYtscToolUsedBrowserChannel").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：因頻道名稱長度超過 {0} 列，故取消裁切截圖。
    /// </summary>
    public static readonly string MsgYtscToolChNameTooLongCancelCut = Application.Current
        .FindResource("MsgYtscToolChNameTooLongCancelCut").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已完成截圖的拍攝。
    /// </summary>
    public static readonly string MsgYtscToolTakeScreenshotFinished = Application.Current
        .FindResource("MsgYtscToolTakeScreenshotFinished").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：截圖檔案位於：{0}
    /// </summary>
    public static readonly string MsgYtscToolFileSaveAt = Application.Current
        .FindResource("MsgYtscToolFileSaveAt").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：訂閱者數字已超過可以顯示的範圍，故不將截圖裁切成正方形。
    /// </summary>
    public static readonly string MsgYtscToolSubscribersTooLongCancelClip = Application.Current
        .FindResource("MsgYtscToolSubscribersTooLongCancelClip").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：Playwright 已結束，離開碼：{0}
    /// </summary>
    public static readonly string MsgPlaywrightException = Application.Current
        .FindResource("MsgPlaywrightException").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：Playwright 瀏覽器的路徑：
    /// </summary>
    public static readonly string MsgPlaywrightBrowserPath = Application.Current
        .FindResource("MsgPlaywrightBrowserPath").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：下載失敗，在短片清單內找不到同網址或影片 ID 的短片。
    /// </summary>
    public static readonly string MsgCantFindTheSameUrlClips = Application.Current
        .FindResource("MsgCantFindTheSameUrlClips").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您是否要下載新版本 v{0}？
    /// </summary>
    public static readonly string MsgUpdateNotifierDownloadNewVersion = Application.Current
        .FindResource("MsgUpdateNotifierDownloadNewVersion").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您是否要下載舊版本 v{0}？
    /// </summary>
    public static readonly string MsgUpdateNotifierDownloadOldVersion = Application.Current
        .FindResource("MsgUpdateNotifierDownloadOldVersion").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤，HTTP 狀態碼 {0}
    /// </summary>
    public static readonly string MsgUpdateNotifierHttpErrorCode = Application.Current
        .FindResource("MsgUpdateNotifierHttpErrorCode").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤，無法解析 AppVersion.json 檔案。
    /// </summary>
    public static readonly string MsgUpdateNotifierCantParsedAppVersionJsonFile = Application.Current
        .FindResource("MsgUpdateNotifierCantParsedAppVersionJsonFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤，找不到 {0} 應用程式的資料。
    /// </summary>
    public static readonly string MsgUpdateNotifierCantFindAppData = Application.Current
        .FindResource("MsgUpdateNotifierCantFindAppData").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤，無法解析 {0} 應用程式的版本號資料。
    /// </summary>
    public static readonly string MsgUpdateNotifierCantParsedAppVersionData = Application.Current
        .FindResource("MsgUpdateNotifierCantParsedAppVersionData").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：有新版本 v{0} 可供下載。
    /// </summary>
    public static readonly string MsgUpdateNotifierHasNewVersion = Application.Current
        .FindResource("MsgUpdateNotifierHasNewVersion").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：您使用的是最新版本 v{0}。
    /// </summary>
    public static readonly string MsgUpdateNotifierAlreadyLatestVersion = Application.Current
        .FindResource("MsgUpdateNotifierAlreadyLatestVersion").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：注意！網路的版本（v{0}）比本機的版本（v{1}）還要舊，請注意是否有降版的公告資訊。
    /// </summary>
    public static readonly string MsgUpdateNotifierDowngradeNotice = Application.Current
        .FindResource("MsgUpdateNotifierDowngradeNotice").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：短片清單無資料。
    /// </summary>
    public static readonly string MsgClipListNoData = Application.Current
        .FindResource("MsgClipListNoData").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：下載成功。
    /// </summary>
    public static readonly string MsgYtDlpDownloadSucceed = Application.Current
        .FindResource("MsgYtDlpDownloadSucceed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：下載失敗。
    /// </summary>
    public static readonly string MsgYtDlpDownloadFailure = Application.Current
        .FindResource("MsgYtDlpDownloadFailure").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已切換至短片播放器模式。
    /// </summary>
    public static readonly string MsgSwitchToClipPlayerMode = Application.Current
        .FindResource("MsgSwitchToClipPlayerMode").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已切換至時間標記編輯器模式。
    /// </summary>
    public static readonly string MsgSwitchToTimestampEditorMode = Application.Current
        .FindResource("MsgSwitchToTimestampEditorMode").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請先切換至時間標記編輯器模式。
    /// </summary>
    public static readonly string MsgSwitchToTimestampEditorModeFirst = Application.Current
        .FindResource("MsgSwitchToTimestampEditorModeFirst").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請先播放短片。
    /// </summary>
    public static readonly string MsgPlayAClipFirst = Application.Current
        .FindResource("MsgPlayAClipFirst").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：未載入 libmpv。
    /// </summary>
    public static readonly string MsgLibMpvIsNotLoaded = Application.Current
        .FindResource("MsgLibMpvIsNotLoaded").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：未載入多媒體。
    /// </summary>
    public static readonly string MsgMediaIsNotLoaded = Application.Current
        .FindResource("MsgMediaIsNotLoaded").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：模型檔案 {0} 不存在，正在開始下載該模型檔案……
    /// </summary>
    public static readonly string MsgWhisperModelIsNotExists = Application.Current
        .FindResource("MsgWhisperModelIsNotExists").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已下載模型檔案 {0}。
    /// </summary>
    public static readonly string MsgWhisperModelIsDownloaded = Application.Current
        .FindResource("MsgWhisperModelIsDownloaded").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已找到模型檔案 {0}。
    /// </summary>
    public static readonly string MsgWhisperModelIsFound = Application.Current
        .FindResource("MsgWhisperModelIsFound").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消轉譯作業。
    /// </summary>
    public static readonly string MsgWhisperTranscribeCanceled = Application.Current
        .FindResource("MsgWhisperTranscribeCanceled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤：請選擇有效的視訊或音訊檔案。
    /// </summary>
    public static readonly string MsgSelectAValidVideoOrAudioFile = Application.Current
        .FindResource("MsgSelectAValidVideoOrAudioFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消作業。
    /// </summary>
    public static readonly string MsgJobHasCanceled = Application.Current
        .FindResource("MsgJobHasCanceled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：發生錯誤：使用的模型檔案不存在或下載失敗。
    /// </summary>
    public static readonly string MsgWhisperModelFileNotFound = Application.Current
        .FindResource("MsgWhisperModelFileNotFound").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已取消偵測語言作業。
    /// </summary>
    public static readonly string MsgWhisperDetectLanguageCanceled = Application.Current
        .FindResource("MsgWhisperDetectLanguageCanceled").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請自行至「{0}」刪除暫存檔案。
    /// </summary>
    public static readonly string MsgWhisperRemoveTempFileByYourSelf = Application.Current
        .FindResource("MsgWhisperRemoveTempFileByYourSelf").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：正在開始偵測語言作業……
    /// </summary>
    public static readonly string MsgWhisperDetectLanguageStarting = Application.Current
        .FindResource("MsgWhisperDetectLanguageStarting").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：使用的模型：{0}
    /// </summary>
    public static readonly string MsgWhisperUsedModel = Application.Current
        .FindResource("MsgWhisperUsedModel").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：使用的量化：{0}
    /// </summary>
    public static readonly string MsgWhisperUsedQuantizationType = Application.Current
        .FindResource("MsgWhisperUsedQuantizationType").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：識別失敗。
    /// </summary>
    public static readonly string MsgWhisperDetectLanguageFailed = Application.Current
        .FindResource("MsgWhisperDetectLanguageFailed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：偵測語言結果：{0}
    /// </summary>
    public static readonly string MsgWhisperDetectLanguageResult = Application.Current
        .FindResource("MsgWhisperDetectLanguageResult").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：總共耗時：{0}
    /// </summary>
    public static readonly string MsgWhisperTotalElapsed = Application.Current
        .FindResource("MsgWhisperTotalElapsed").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已刪除暫時檔案：{0}
    /// </summary>
    public static readonly string MsgWhisperTempFileDeleted = Application.Current
        .FindResource("MsgWhisperTempFileDeleted").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：正在開始轉譯作業……
    /// </summary>
    public static readonly string MsgWhisperTranscribeStarting = Application.Current
        .FindResource("MsgWhisperTranscribeStarting").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：使用的語言：{0}
    /// </summary>
    public static readonly string MsgWhisperTranscribeUsedLanguage = Application.Current
        .FindResource("MsgWhisperTranscribeUsedLanguage").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：使用的抽樣策略：{0}
    /// </summary>
    public static readonly string MsgWhisperTranscribeUsedSamplingStrategyType = Application.Current
        .FindResource("MsgWhisperTranscribeUsedSamplingStrategyType").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：啟用 OpenCC S2TWP：{0}
    /// </summary>
    public static readonly string MsgWhisperTranscribeEnableOpenCCS2TWP = Application.Current
        .FindResource("MsgWhisperTranscribeEnableOpenCCS2TWP").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：轉譯的內容：
    /// </summary>
    public static readonly string MsgWhisperTranscribeContent = Application.Current
        .FindResource("MsgWhisperTranscribeContent").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：轉譯完成。
    /// </summary>
    public static readonly string MsgWhisperTranscribeFinished = Application.Current
        .FindResource("MsgWhisperTranscribeFinished").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：開始建立 {0} 字幕檔……
    /// </summary>
    public static readonly string MsgWhisperStartToCreateSubtitleFile = Application.Current
        .FindResource("MsgWhisperStartToCreateSubtitleFile").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已建立 {0} 字幕檔：{1}
    /// </summary>
    public static readonly string MsgWhisperSubtitleFileCreated = Application.Current
        .FindResource("MsgWhisperSubtitleFileCreated").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：請選擇 Bilibili 的播放清單。
    /// </summary>
    public static readonly string MsgSelectB23Playlist = Application.Current
        .FindResource("MsgSelectB23Playlist").ToString() ??
        string.Empty;

    /// <summary>
    /// 訊息：已更新 Sec-CH-UA。
    /// </summary>
    public static readonly string MsgUpdateSecChUa = Application.Current
        .FindResource("MsgUpdateSecChUa").ToString() ??
        string.Empty;

    #endregion

    /// <summary>
    /// 取得格式化的字串
    /// </summary>
    /// <param name="format">字串，格式</param>
    /// <param name="value">字串陣列，值</param>
    /// <returns>字串</returns>
    public static string GetFmtStr(
        string? format,
        params string[] value)
    {
        if (string.IsNullOrEmpty(format))
        {
            return string.Join(string.Empty, value);
        }

        return string.Format(format, value);
    }
}