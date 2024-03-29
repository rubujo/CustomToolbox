﻿using Application = System.Windows.Application;
using CustomToolbox.BilibiliApi.Functions;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Sets;
using Downloader;
using Humanizer;
using Label = System.Windows.Controls.Label;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Serilog.Events;
using SevenZipExtractor;
using System.IO;
using System.Net;

using System.Net.Http;

namespace CustomToolbox.Common.Utils;

/// <summary>
/// 下載器工具
/// </summary>
public class DownloaderUtil
{
    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

    /// <summary>
    /// PBProgress
    /// </summary>
    private static ProgressBar? _PBProgress = null;

    /// <summary>
    /// LOperation
    /// </summary>
    private static Label? _LOperation = null;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="wMain">WMain</param>
    public static void Init(WMain wMain)
    {
        _WMain = wMain;
        _PBProgress = wMain.PBProgress;
        _LOperation = wMain.LOperation;
    }

    /// <summary>
    /// 下載 yt-dlp
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadYtDlp()
    {
        try
        {
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgStartDownloading,
                    VariableSet.YtDlpExecName));

            string path = Path.Combine(
                VariableSet.BinsFolderPath,
                VariableSet.YtDlpExecName);

            DownloadService downloadService = GetDownloadService(
                action: new Action(() =>
                {
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgDownloaded,
                            VariableSet.YtDlpExecName));

                    // 在下載後寫入檢查時間，以避免不必要的檢查執行。
                    Properties.Settings.Default.YtDlpCheckTime = DateTime.Now;
                    Properties.Settings.Default.Save();

                    ExternalProgram.SetYtDlpVersion();
                }));

            await downloadService.DownloadFileTaskAsync(UrlSet.YtDlpUrl, path);
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
    /// 下載 yt-dlp/FFmpeg-Builds
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadFFmpeg()
    {
        try
        {
            string path = Path.Combine(
                VariableSet.BinsFolderPath,
                UrlSet.FFmpegArchiveFileName);

            DownloadService downloadService = GetDownloadService(
                action: new Action(() =>
                {
                    if (File.Exists(path))
                    {
                        using ArchiveFile archiveFile = new(path);

                        IList<Entry> entries = archiveFile.Entries
                                    .Where(n => !n.IsFolder &&
                                        (n.FileName.Contains(VariableSet.FFmpegExecName) ||
                                        n.FileName.Contains(VariableSet.FFprobeExecName))).ToList();

                        foreach (Entry entry in entries)
                        {
                            string fileName = Path.GetFileName(entry.FileName);
                            string targetPath = Path.Combine(VariableSet.BinsFolderPath, fileName);

                            entry.Extract(targetPath);

                            _WMain?.WriteLog(
                                message: MsgSet.GetFmtStr(
                                    MsgSet.MsgDecompressed,
                                    fileName));
                        }

                        Task.Delay(VariableSet.WaitForDeleteMilliseconds)
                            .ContinueWith(t => File.Delete(path))
                            .ContinueWith(t => _WMain?.WriteLog(
                                MsgSet.GetFmtStr(
                                    MsgSet.MsgDeleted,
                                    path)));
                    }
                }));

            await downloadService.DownloadFileTaskAsync(UrlSet.FFmpegUrl, path);
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
    /// 下載 sub_charenc_parameters.txt
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadSubCharencParametersTxt()
    {
        try
        {
            DownloadService downloadService = GetDownloadService();

            if (File.Exists(VariableSet.SubCharencParametersTxtPath))
            {
                File.Delete(VariableSet.SubCharencParametersTxtPath);

                _WMain?.WriteLog(MsgSet.GetFmtStr(
                    MsgSet.MsgDeleted,
                    VariableSet.SubCharencParametersTxtFileName));
            }

            await downloadService.DownloadFileTaskAsync(
                UrlSet.SubCharencParametersTxtUrl,
                VariableSet.SubCharencParametersTxtPath);
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
    /// 下載 aria2
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadAria2()
    {
        try
        {
            string path = Path.Combine(
                VariableSet.BinsFolderPath,
                UrlSet.Aria2ArchiveFileName);

            DownloadService downloadService = GetDownloadService(
                action: new Action(() =>
                {
                    if (File.Exists(path))
                    {
                        using ArchiveFile archiveFile = new(path);

                        IList<Entry> entries = archiveFile.Entries
                            .Where(n => !n.IsFolder &&
                                n.FileName.Contains(VariableSet.Aria2ExecName)).ToList();

                        foreach (Entry entry in entries)
                        {
                            string fileName = Path.GetFileName(entry.FileName);
                            string targetPath = Path.Combine(VariableSet.BinsFolderPath, fileName);

                            entry.Extract(targetPath);

                            _WMain?.WriteLog(MsgSet.GetFmtStr(
                                MsgSet.MsgDecompressed,
                                fileName));
                        }

                        Task.Delay(VariableSet.WaitForDeleteMilliseconds)
                            .ContinueWith(t => File.Delete(path))
                            .ContinueWith(t => _WMain?.WriteLog(
                                MsgSet.GetFmtStr(
                                    MsgSet.MsgDeleted,
                                    path)));
                    }
                }));

            await downloadService.DownloadFileTaskAsync(UrlSet.Aria2Url, path);
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
    /// 下載 libmpv
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadLibMpv()
    {
        try
        {
            string path = Path.Combine(
                VariableSet.BinsFolderPath,
                UrlSet.LibMpvArchiveFileName);

            DownloadService downloadService = GetDownloadService(
                // 因帶 UserAgent 會造成下載失敗，故使用預設的 DownloadConfiguration。
                downloadConfiguration: new DownloadConfiguration(),
                action: new Action(() =>
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            using ArchiveFile archiveFile = new(path);

                            IList<Entry> entries = archiveFile.Entries
                                    .Where(n => !n.IsFolder &&
                                        n.FileName.Contains(VariableSet.LibMpvDllFileName)).ToList();

                            foreach (Entry entry in entries)
                            {
                                string fileName = Path.GetFileName(entry.FileName);
                                string targetPath = Path.Combine(VariableSet.BinsFolderPath, fileName);

                                entry.Extract(targetPath);

                                _WMain?.WriteLog(
                                    message: MsgSet.GetFmtStr(
                                        MsgSet.MsgDecompressed,
                                        fileName));
                            }

                            Task.Delay(VariableSet.WaitForDeleteMilliseconds)
                                    .ContinueWith(t => File.Delete(path))
                                    .ContinueWith(t => _WMain?.WriteLog(
                                        MsgSet.GetFmtStr(
                                            MsgSet.MsgDeleted,
                                            path)));
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
                }));

            await downloadService.DownloadFileTaskAsync(UrlSet.LibMpvUrl, path);
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
    /// 下載 ytdl_hook.lua
    /// </summary>
    /// <returns>Task</returns>
    public static async Task DownloadYtDlHookLua()
    {
        try
        {
            DownloadService downloadService = GetDownloadService(
                action: new Action(() =>
                {
                    if (File.Exists(VariableSet.YtDlHookLuaPath))
                    {
                        // 修改「ytdl_path = "",」。
                        string[] lines = File.ReadAllLines(VariableSet.YtDlHookLuaPath);

                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[i];

                            // 判斷是否為目標行。
                            if (line.StartsWith("    ytdl_path = \"\","))
                            {
                                string folderName = new DirectoryInfo(VariableSet.BinsFolderPath).Name;

                                // 替換內容。
                                line = $"    ytdl_path = \"{folderName}\\\\yt-dlp\",";

                                // 回寫至陣列。
                                lines[i] = line;
                            }
                        }

                        // 將陣列輸出至指定檔案內。
                        File.WriteAllText(
                            VariableSet.YtDlHookLuaPath,
                            string.Join(Environment.NewLine, lines));

                        _WMain?.WriteLog(
                            message: MsgSet.GetFmtStr(
                                MsgSet.MsgEdited,
                                VariableSet.YtDlHookLuaFileName));
                    }
                }));

            if (File.Exists(VariableSet.YtDlHookLuaPath))
            {
                File.Delete(VariableSet.YtDlHookLuaPath);

                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgDeleted,
                        VariableSet.YtDlHookLuaFileName));
            }

            await downloadService.DownloadFileTaskAsync(
                UrlSet.YtDlHookLuaUrl,
                VariableSet.YtDlHookLuaPath);
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
    /// 取得 DownloadService
    /// </summary>
    /// <param name="downloadConfiguration">DownloadConfiguration，預設值是 null</param>
    /// <param name="action">Action，預設值是 null</param>
    /// <returns>DownloadService</returns>
    public static DownloadService GetDownloadService(
        DownloadConfiguration? downloadConfiguration = null,
        Action? action = null)
    {
        if (downloadConfiguration == null)
        {
            downloadConfiguration = new DownloadConfiguration();

            // 當使用者代理字串值不為空時才設定。
            if (!string.IsNullOrEmpty(Properties.Settings.Default.UserAgent))
            {
                downloadConfiguration.RequestConfiguration = new RequestConfiguration()
                {
                    UserAgent = CustomFunction.GetUserAgent()
                };
            }
            else
            {
                // 使用預設的 RequestConfiguration。
                downloadConfiguration.RequestConfiguration = new RequestConfiguration();
            }
        }

        // 供手動減速使用。
        int previousPercentageValue = 0;

        string fileName = string.Empty;

        DownloadService downloadService = new(downloadConfiguration);

        downloadService.DownloadStarted += (sender, e) =>
        {
            fileName = Path.GetFileName(e.FileName);

            if (_PBProgress != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _PBProgress.Maximum = 100;
                }));
            }

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgStartDownloading,
                    fileName));
        };

        downloadService.DownloadProgressChanged += (sender, e) =>
        {
            int currentPercentageValue = (int)e.ProgressPercentage;

            // 減速更新 UI 的頻率，以免 UI 卡死。
            if (currentPercentageValue > previousPercentageValue)
            {
                // 更新 UI。
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_PBProgress != null)
                    {
                        _PBProgress.Value = currentPercentageValue;
                        _PBProgress.ToolTip = $"{currentPercentageValue}%";
                    }

                    if (_LOperation != null)
                    {
                        string stringValue = $"[{e.ProgressId}] " +
                            $"{e.ReceivedBytesSize.Bytes().LargestWholeNumberValue:#.##}/" +
                            $"{e.TotalBytesToReceive.Bytes()} " +
                            $"({currentPercentageValue}%) " +
                            $"{e.BytesPerSecondSpeed.Bytes()}/s";

                        _LOperation.Content = stringValue;
                        _LOperation.ToolTip = stringValue;
                    }
                }));
            }

            previousPercentageValue = currentPercentageValue;
        };

        downloadService.DownloadFileCompleted += (sender, e) =>
        {
            // 重設 previousPercentageValue。
            previousPercentageValue = 0;

            // 重設 UI。
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_PBProgress != null)
                {
                    _PBProgress.Value = 0;
                    _PBProgress.ToolTip = string.Empty;
                }

                if (_LOperation != null)
                {
                    _LOperation.Content = string.Empty;
                    _LOperation.ToolTip = string.Empty;
                }
            }));

            if (e.Cancelled)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    _WMain?.WriteLog(message: MsgSet.MsgDownloadCanceled);
                }
                else
                {
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgCancelFileDownload,
                            fileName));
                }
            }
            else if (e.Error != null)
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.DownloadError,
                         e.Error.Message),
                    logEventLevel: LogEventLevel.Error);
            }
            else
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    _WMain?.WriteLog(message: MsgSet.MsgDownloadFinished);
                }
                else
                {
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgDownloaded,
                            fileName));
                }

                action?.Invoke();
            }

            // 重設 fileName。
            fileName = string.Empty;
        };

        return downloadService;
    }

    /// <summary>
    /// 取得針對 Bilibili 網站使用的 DownloadConfiguration
    /// </summary>
    /// <param name="httpClient">HttpClient，預設值是 null</param>
    /// <returns>DownloadConfiguration</returns>
    public static async Task<DownloadConfiguration> GetB23DownloadConfiguration(
        HttpClient? httpClient = null)
    {
        string strB_3 = string.Empty;

        if (httpClient != null)
        {
            // 2023/9/22 Bilibili Buvid3 參考來源：
            // https://github.com/SocialSisterYi/bilibili-API-collect/issues/788
            // https://github.com/SocialSisterYi/bilibili-API-collect/issues/790
            // https://github.com/SocialSisterYi/bilibili-API-collect/issues/795
            (strB_3, string _) = await AuthFunction.GetBuvids(httpClient);
        }

        DownloadConfiguration downloadConfiguration = new();

        WebHeaderCollection headerCollection = new()
        {
            { "Origin", "https://space.bilibili.com" },
            { "DNT", "1" }
        };

        if (!string.IsNullOrEmpty(strB_3))
        {
            headerCollection.Add("Cookie", $"buvid3={strB_3};");
        }

        ClientHintsUtil.SetClientHints(headerCollection);

        // TODO: 2023/12/27 因應 -352 風險校驗失敗。（待持續觀察）
        // 參考：https://github.com/SocialSisterYi/bilibili-API-collect/issues/868

        // 當使用者代理字串值不為空時才設定。
        if (!string.IsNullOrEmpty(Properties.Settings.Default.UserAgent))
        {
            downloadConfiguration.RequestConfiguration = new RequestConfiguration()
            {
                // 來源：https://github.com/Nemo2011/bilibili-api/issues/595#issuecomment-1859074892
                UserAgent = "Mozilla/5.0",
                Referer = "https://www.bilibili.com",
                Headers = headerCollection
            };
        }
        else
        {
            // 使用預設的 RequestConfiguration。
            downloadConfiguration.RequestConfiguration = new RequestConfiguration()
            {
                // 來源：https://github.com/Nemo2011/bilibili-api/issues/595#issuecomment-1859074892
                UserAgent = "Mozilla/5.0",
                Referer = "https://www.bilibili.com",
                Headers = headerCollection
            };
        }

        return downloadConfiguration;
    }
}