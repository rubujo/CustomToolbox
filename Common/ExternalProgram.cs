using Application = System.Windows.Application;
using CustomToolbox.Common.Extensions;
using static CustomToolbox.Common.Sets.EnumSet;
using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Utils;
using Label = System.Windows.Controls.Label;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Serilog.Events;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Whisper.net.Ggml;
using Xabe.FFmpeg.Events;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace CustomToolbox.Common;

/// <summary>
/// 外部程式
/// </summary>
public class ExternalProgram
{
    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

    /// <summary>
    /// LYtDlpVersion
    /// </summary>
    private static Label? _LYtDlpVersion = null;

    /// <summary>
    /// LOperation
    /// </summary>
    private static Label? _LOperation = null;

    /// <summary>
    /// PBProgress
    /// </summary>
    private static ProgressBar? _PBProgress = null;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="wMain">WMain</param>
    public static void Init(WMain wMain)
    {
        _WMain = wMain;
        _LYtDlpVersion = _WMain.LYtDlpVersion;
        _LOperation = _WMain.LOperation;
        _PBProgress = _WMain.PBProgress;
    }

    /// <summary>
    /// 檢查相依性檔案
    /// </summary>
    /// <param name="action">Action</param>
    /// <param name="isForceDownload">布林值，是否強制下載，預設值為 false</param>
    public static async void CheckDependencyFiles(Action? action, bool isForceDownload = false)
    {
        try
        {
            #region 檢查資料夾

            string[] folders =
            [
                VariableSet.BinsFolderPath,
                VariableSet.DownloadsFolderPath,
                VariableSet.ClipListsFolderPath,
                VariableSet.LogsFolderPath,
                VariableSet.LyricsFolderPath,
                VariableSet.TempFolderPath,
                VariableSet.ModelsFolderPath
            ];

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);

                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgCreated,
                            folder));
                }
            }

            #endregion

            #region 檢查 yt-dlp

            if (!File.Exists(VariableSet.YtDlpPath) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadYtDlp();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.YtDlpExecName));

                // 當今天的日期晚於設定檔中的日期時。
                if (DateTime.Now.Date > Properties.Settings.Default.YtDlpCheckTime.Date)
                {
                    UpdateYtDlp();
                }
                else
                {
                    SetYtDlpVersion();

                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgYtDlpNotice,
                            $"{Properties.Settings.Default.YtDlpCheckTime:yyyy/MM/dd HH:mm:ss}"));
                }
            }

            #endregion

            #region 檢查 yt-dlp.conf

            if (!File.Exists(VariableSet.YtDlpConfPath) ||
                isForceDownload)
            {
                GetOptionSet(forceCreate: true);
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.YtDlpConfName));
            }

            #endregion

            #region 檢查 FFmpeg

            if (!File.Exists(VariableSet.FFmpegPath) ||
                !File.Exists(VariableSet.FFprobePath) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadFFmpeg();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        $"{VariableSet.FFmpegExecName}, " +
                        $"{VariableSet.FFprobeExecName}"));
            }

            #endregion

            #region 檢查 sub_charenc_parameters.txt

            if (!File.Exists(VariableSet.SubCharencParametersTxtPath) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadSubCharencParametersTxt();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.SubCharencParametersTxtFileName));
            }

            #endregion

            #region 檢查 aria2

            if (!File.Exists(VariableSet.Aria2Path) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadAria2();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.Aria2ExecName));
            }

            #endregion

            #region 檢查 mpv.conf

            if (!File.Exists(VariableSet.MpvConfPath) ||
                isForceDownload)
            {
                CreateDefaultMpvConf();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.MpvConfFileName));
            }

            #endregion

            #region 檢查 libmpv

            if (!File.Exists(VariableSet.LibMpvPath) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadLibMpv();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.LibMpvDllFileName));
            }

            #endregion

            #region 檢查 ytdl_hook.lua

            if (!File.Exists(VariableSet.YtDlHookLuaPath) ||
                isForceDownload)
            {
                await DownloaderUtil.DownloadYtDlHookLua();
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgFound,
                        VariableSet.YtDlHookLuaFileName));
            }

            #endregion

            action?.Invoke();
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
    /// 更新 yt-dlp
    /// </summary>
    public static async void UpdateYtDlp()
    {
        YoutubeDL ytdl = GetYoutubeDL();

        string result = await ytdl.RunUpdate();

        _WMain?.WriteLog(result);

        // 當 yt-dlp 更新後，才需要提示要更新 FFmpeg。
        if (result.Contains("Updated yt-dlp to"))
        {
            _WMain?.WriteLog(MsgSet.MsgUpdateFFmpeg);
        }

        Properties.Settings.Default.YtDlpCheckTime = DateTime.Now;
        Properties.Settings.Default.Save();

        SetYtDlpVersion();
    }

    /// <summary>
    /// 更新 yt-dlp 至
    /// <para>自定義標籤的範例：stable@2023.07.06</para>
    /// </summary>
    /// <param name="ytDlpUpdateChannelType">YtDlpUpdateChannelType，預設值為 YtDlpUpdateChannelType.Stable</param>
    /// <param name="customTag">字串，自定義的標籤，預設值為空白</param>
    public static async void UpdateYtDlpTo(
        YtDlpUpdateChannelType ytDlpUpdateChannelType = YtDlpUpdateChannelType.Stable,
        string customTag = "")
    {
        YoutubeDL ytdl = GetYoutubeDL();

        string result = await ytdl.RunUpdateTo(ytDlpUpdateChannelType, customTag);

        // 2023/12/1 
        // yt-dlp 並不會真的因為使用 --update-to 就可以隨意跨頻道降版本，
        // 當目標頻道的版本號低於目前的頻道版本號，就不會降版，需要自定義版本號才會降版。

        string[] arrayMessages = result.Split('｜', StringSplitOptions.RemoveEmptyEntries);

        // 判斷訊息在拆分後是否有多於或等於 2 則訊息。
        if (arrayMessages.Length >= 2)
        {
            // 取得目前的版本號。
            string? strCurrentVsrsion = (arrayMessages
                .FirstOrDefault(n => n.Contains("yt-dlp is up to date ("))
                ?.Replace("yt-dlp is up to date (", string.Empty))
                ?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()
                ?.Trim() ??
                string.Empty,
                // 取得目標的版本號。
                strTargetVersion = (arrayMessages
                    .FirstOrDefault(n => n.Contains("Latest version: "))
                    ?.Replace("Latest version: ", string.Empty))
                    ?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim() ??
                    string.Empty;

            // 判斷兩個版本號是否一致。
            if (!string.IsNullOrEmpty(strTargetVersion) &&
                !string.IsNullOrEmpty(strCurrentVsrsion) &&
                strTargetVersion != strCurrentVsrsion)
            {
                // 2023/12/1 輸出兩個版本號。
                _WMain?.WriteLog(strCurrentVsrsion);
                _WMain?.WriteLog(strTargetVersion);

                // 再次執行一次 UpdateYtDlpTo() 方法。
                UpdateYtDlpTo(YtDlpUpdateChannelType.Custom, strTargetVersion);

                return;
            }
        }

        // 只輸出最後（最新）一筆的訊息。
        result = arrayMessages.LastOrDefault() ?? string.Empty;

        _WMain?.WriteLog(result);

        // 當 yt-dlp 更新後，才需要提示要更新 FFmpeg。
        if (result.Contains("Updated yt-dlp to"))
        {
            _WMain?.WriteLog(MsgSet.MsgUpdateFFmpeg);
        }

        Properties.Settings.Default.YtDlpCheckTime = DateTime.Now;
        Properties.Settings.Default.Save();

        SetYtDlpVersion();
    }

    /// <summary>
    /// 設定顯示 yt-dlp 的版本號
    /// </summary>
    public static void SetYtDlpVersion()
    {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            YoutubeDL ytdl = GetYoutubeDL();

            if (_LYtDlpVersion != null)
            {
                _LYtDlpVersion.Content = ytdl.Version;
            }
        }));
    }

    /// <summary>
    /// 取得 YoutubeDL
    /// </summary>
    /// <returns>YoutubeDL</returns>
    public static YoutubeDL GetYoutubeDL()
    {
        return new YoutubeDL()
        {
            FFmpegPath = VariableSet.FFmpegPath,
            IgnoreDownloadErrors = true,
            OutputFolder = VariableSet.DownloadsFolderPath,
            OverwriteFiles = true,
            RestrictFilenames = false,
            YoutubeDLPath = VariableSet.YtDlpPath,
        };
    }

    /// <summary>
    /// 建立預設的 mpv.conf
    /// </summary>
    public static void CreateDefaultMpvConf()
    {
        if (File.Exists(VariableSet.MpvConfPath))
        {
            File.Delete(VariableSet.MpvConfPath);

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgDeleted,
                    VariableSet.MpvConfFileName));
        }

        using StreamWriter streamWriter = new(VariableSet.MpvConfPath, false);

        streamWriter.Write(VariableSet.MpvConfTemplate);

        _WMain?.WriteLog(
            message: MsgSet.GetFmtStr(
                MsgSet.MsgCreated,
                VariableSet.MpvConfFileName));
    }

    /// <summary>
    /// 取得 OptionSet
    /// </summary>
    /// <param name="forceCreate">布林值，是否強制建立，預設值為 false</param>
    /// <returns>OptionSet</returns>
    public static OptionSet GetOptionSet(bool forceCreate = false)
    {
        if (!File.Exists(VariableSet.YtDlpConfPath) ||
            forceCreate)
        {
            if (File.Exists(VariableSet.YtDlpConfPath))
            {
                File.Delete(VariableSet.YtDlpConfPath);

                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgDeleted,
                        VariableSet.YtDlpConfName));
            }

            OptionSet optionSet = new()
            {
                ConcurrentFragments = Properties.Settings.Default.YtDlpConcurrentFragments,
                FfmpegLocation = VariableSet.FFmpegPath,
                ForceKeyframesAtCuts = false,
                Format = Properties.Settings.Default.YtDlpDefaultFormat,
                Output = VariableSet.YtDlpDefaultOutput,
                RemoveCacheDir = true,
                SkipUnavailableFragments = true,
                SubLangs = Properties.Settings.Default.YtDlpSubLangs
            };

            optionSet.WriteConfigFile(VariableSet.YtDlpConfPath);

            _WMain?.WriteLog(message: MsgSet.GetFmtStr(
                MsgSet.MsgCreated,
                VariableSet.YtDlpConfName));
        }

        OptionSet loadedOptionSet = OptionSet
            .LoadConfigFile(VariableSet.YtDlpConfPath);

        #region 每次載入時都更新一次相關的路徑

        loadedOptionSet.FfmpegLocation = VariableSet.FFmpegPath;
        loadedOptionSet.Output = VariableSet.YtDlpDefaultOutput;

        if (loadedOptionSet.DownloaderArgs?.Values?.Contains("aria2c:--allow-overwrite=true") == true)
        {
            loadedOptionSet.Downloader = new MultiValue<string>(VariableSet.Aria2Path);
        }    

        loadedOptionSet.WriteConfigFile(VariableSet.YtDlpConfPath);

        #endregion

        return loadedOptionSet;
    }

    /// <summary>
    /// 取得設定過的 OptionSet
    /// </summary>
    /// <param name="startSeconds">數值，開始秒數，預設值為 0.0d</param>
    /// <param name="endSeconds">數值，結束秒數，預設值為 0.0d</param>
    /// <param name="endSeconds">字串，檔案名稱，預設值為空白</param>
    /// <param name="isAudioOnly">布林值，是否為僅音訊，預設值為 false</param>
    /// <param name="isFullDownloadFirst">布林值，是否先下載完整短片，預設值為 false</param>
    /// <returns>OptionSet</returns>
    public static OptionSet GetConfiguredOptionSet(
        double startSeconds = 0.0d,
        double endSeconds = 0.0d,
        string? fileName = "",
        bool isAudioOnly = false,
        bool isFullDownloadFirst = false)
    {
        OptionSet optionSet = GetOptionSet();

        // 判斷是否為僅音訊。
        if (isAudioOnly)
        {
            // 限制只載最好音質的 m4a 音訊檔。
            optionSet.Format = "ba[ext=m4a]";
        }

        // 計算間隔秒數。
        double durationSeconds = endSeconds - startSeconds;

        // 判斷是否先下載完整影片。
        if (isFullDownloadFirst)
        {
            // 當 durationSeconds 為正整數時，即表示還有後續操作，
            // 故套用下列設定。
            if (durationSeconds > 0)
            {
                // 為了以利 FFmpeg 的作業，故採用下列設定。
                optionSet.Output = Path.Combine(
                    VariableSet.DownloadsFolderPath,
                    $"%(id)s_{DateTime.Now:yyyyMMddHHmmss}.%(ext)s");
                optionSet.EmbedMetadata = false;
                optionSet.EmbedThumbnail = false;
                optionSet.EmbedSubs = false;
                optionSet.SplitChapters = false;
            }
            else
            {
                // 當 fileName 不為空白或 null 時才可以覆寫 Output。
                if (!string.IsNullOrEmpty(fileName))
                {
                    optionSet.Output = Path.Combine(
                        VariableSet.DownloadsFolderPath,
                        $"%(id)s_{fileName}_{DateTime.Now:yyyyMMddHHmmss}.%(ext)s");
                }
            }
        }
        else
        {
            // 當 durationSeconds 為正整數時，
            // 則使用 FFmpeg 作為外部下載器來下載短片片段。
            if (durationSeconds > 0)
            {
                // 當 fileName 不為空白或 null 時才可以覆寫 Output。
                if (!string.IsNullOrEmpty(fileName))
                {
                    optionSet.Output = Path.Combine(
                        VariableSet.DownloadsFolderPath,
                        $"%(id)s_{fileName}_{DateTime.Now:yyyyMMddHHmmss}.%(ext)s");
                }

                // 參考來源：https://www.reddit.com/r/youtubedl/wiki/howdoidownloadpartsofavideo/
                optionSet.Downloader = "ffmpeg";
                optionSet.DownloaderArgs = $"ffmpeg_i:-ss {startSeconds} -to {endSeconds}";
                optionSet.ForceKeyframesAtCuts = true;
                // 因為需要透過 FFmpeg 再處裡，所以要取消下列設定。
                optionSet.EmbedMetadata = false;
                optionSet.EmbedThumbnail = false;
                optionSet.EmbedSubs = false;
                optionSet.SplitChapters = false;
            }
        }

        return optionSet;
    }

    /// <summary>
    /// 取得 IConversion
    /// </summary>
    /// <param name="mediaInfo">IMediaInfo</param>
    /// <param name="startSeconds">數值，開始秒數</param>
    /// <param name="endSeconds">數值，結束秒數</param>
    /// <param name="output">字串，檔案輸出的路徑</param>
    /// <param name="fixDuration">布林值，是否執行使用步驟，預設值為 false</param>
    /// <param name="useCodecCopy">布林值，是否使用 -c:v copy -c:a copy，預設值為 true</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">EnumSet.HardwareAcceleratorType，硬體的類型，預設是 EnumSet.HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isAudioOnly">布林值，是否僅音訊，預設為 false</param>
    /// <returns>IConversion</returns>
    public static IConversion GetConversion(
        IMediaInfo mediaInfo,
        double startSeconds,
        double endSeconds,
        string output,
        bool fixDuration = false,
        bool useCodecCopy = true,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isAudioOnly = false)
    {
        IConversion conversion = FFmpeg.Conversions.New()
            // 來源：https://trac.ffmpeg.org/wiki/Encode/YouTube
            // 因會產生錯誤，故取消。
            //.AddParameter("-pix_fmt yuv420p")
            .AddParameter("-movflags +faststart")
            .SetOutput(output)
            .SetOverwriteOutput(true);

        if (useHardwareAcceleration)
        {
            SetHardwareAccelerator(
                conversion,
                hardwareAcceleratorType,
                deviceNo);
        }

        if (fixDuration)
        {
            double durationSeconds = startSeconds - endSeconds;

            conversion.AddParameter($"-sseof {durationSeconds}", ParameterPosition.PreInput);

            if (!isAudioOnly)
            {
                conversion.AddStream(mediaInfo.VideoStreams);
            }

            conversion.AddStream(mediaInfo.AudioStreams);
        }
        else
        {
            List<IStream> streams = [];

            if (!isAudioOnly)
            {
                foreach (VideoStream stream in mediaInfo.VideoStreams.Cast<VideoStream>())
                {
                    if (useCodecCopy)
                    {
                        streams.Add(stream.SetCodec(VideoCodec.copy));
                    }
                    else
                    {
                        streams.Add(stream);
                    }
                }
            }

            foreach (AudioStream stream in mediaInfo.AudioStreams.Cast<AudioStream>())
            {
                if (useCodecCopy)
                {
                    streams.Add(stream.SetCodec(AudioCodec.copy));
                }
                else
                {
                    streams.Add(stream);
                }
            }

            conversion.AddStream(streams)
                .SetSeek(TimeSpan.FromSeconds(startSeconds))
                .SetInputTime(TimeSpan.FromSeconds(endSeconds));
        }

        conversion.OnDataReceived += Conversion_OnDataReceived;
        conversion.OnProgress += Conversion_OnProgress;

        return conversion;
    }

    /// <summary>
    /// 取得燒錄字幕的 IConversion
    /// </summary>
    /// <param name="mediaInfo">IMediaInfo，影片的資訊</param>
    /// <param name="subtitlePath">字串，字幕檔案的路徑</param>
    /// <param name="outputPath">字串，輸出檔案的路徑</param>
    /// <param name="encoding">字串，字幕檔的文字編碼，預設值為 "UTF-8"</param>
    /// <param name="applyFontSetting">布林值，套用字型設定，預設值為 false</param>
    /// <param name="fontName">字串，字型名稱，預設值為 null</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">EnumSet.HardwareAcceleratorType，硬體的類型，預設是 EnumSet.HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <returns>IConversion</returns>
    public static IConversion GetBurnInSubtitleConversion(
        IMediaInfo mediaInfo,
        string subtitlePath,
        string outputPath,
        string encoding = "UTF-8",
        bool applyFontSetting = false,
        string? fontName = null,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0)
    {
        // 先處理字幕檔案的路徑。
        subtitlePath = subtitlePath.Replace("\\", "\\\\").Replace(":", "\\:");

        string subtitleParam = $"-vf \"subtitles='{subtitlePath}'";

        // 預設使用 UTF-8 文字編碼。
        subtitleParam += applyFontSetting ? $":charenc='{encoding}'" : ":charenc='UTF-8'";

        if (applyFontSetting)
        {
            if (!string.IsNullOrEmpty(fontName))
            {
                subtitleParam += $":force_style='FontName='{fontName}''";
            }
        }

        subtitleParam += "\"";

        IConversion conversion = FFmpeg.Conversions.New()
            .AddParameter("-movflags +faststart")
            .AddStream(mediaInfo.Streams)
            .AddParameter(subtitleParam)
            .SetOutput(outputPath)
            .SetOverwriteOutput(true);

        if (useHardwareAcceleration)
        {
            SetHardwareAccelerator(
                conversion,
                hardwareAcceleratorType,
                deviceNo);
        }

        conversion.OnDataReceived += Conversion_OnDataReceived;
        conversion.OnProgress += Conversion_OnProgress;

        return conversion;
    }

    /// <summary>
    /// 取得加入 ISubtitleStream 的 IConversion
    /// </summary>
    /// <param name="mediaInfo">IMediaInfo，影片的資訊</param>
    /// <param name="subtitleInfo">IMediaInfo，字幕檔的資訊</param>
    /// <param name="outputPath">字串，輸出檔案的路徑</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">EnumSet.HardwareAcceleratorType，硬體的類型，預設是 EnumSet.HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <returns>IConversion</returns>
    public static IConversion GetAddSubtitleStreamConversion(
        IMediaInfo mediaInfo,
        IMediaInfo subtitleInfo,
        string outputPath,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0)
    {
        // 取得 ISubtitleStream。
        IEnumerable<ISubtitleStream> subtitleStream = subtitleInfo.SubtitleStreams;

        IConversion conversion = FFmpeg.Conversions.New()
            .AddParameter("-movflags +faststart")
            .AddStream(mediaInfo.Streams)
            .AddStream(subtitleStream)
            .SetOutput(outputPath)
            .SetOverwriteOutput(true);

        if (useHardwareAcceleration)
        {
            SetHardwareAccelerator(
                conversion,
                hardwareAcceleratorType,
                deviceNo);
        }

        conversion.OnDataReceived += Conversion_OnDataReceived;
        conversion.OnProgress += Conversion_OnProgress;

        return conversion;
    }

    /// <summary>
    /// 取得加入轉換成 WAV 檔案的 IConversion
    /// </summary>
    /// <param name="audioStreams">IEnumerable&lt;IAudioStream&gt;</param>
    /// <param name="outputPath">字串，輸出檔案的路徑</param>
    /// <returns>IConversion</returns>
    public static IConversion GetConvertToWavConversion(
        IEnumerable<IAudioStream> audioStreams,
        string outputPath)
    {
        // 轉換成取樣率為 16 kHz 的 WAV 檔案。
        IConversion conversion = FFmpeg.Conversions.New()
            .AddStream(audioStreams)
            // Source: https://github.com/tigros/Whisperer/blob/dcdbcd8c9b01c06016272e4a6784774768b7b316/whisperer/Form1.cs#L220
            // Author: tigros
            // TODO: 2023/3/20 需要再觀察下列參數適不適合。
            .AddParameter("-vn -ar 16000 -ab 32k -af volume=1.75")
            .SetOutputFormat(Format.wav)
            .SetOutput(outputPath)
            .SetOverwriteOutput(true);

        conversion.OnDataReceived += Conversion_OnDataReceived;
        conversion.OnProgress += Conversion_OnProgress;

        return conversion;
    }

    /// <summary>
    /// 對 IConversion 設定硬體加速相關參數
    /// </summary>
    /// <param name="conversion">IConversion</param>
    /// <param name="hardwareAcceleratorType">EnumSet.HardwareAcceleratorType，硬體的類型，預設是 EnumSet.HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    public static void SetHardwareAccelerator(
        IConversion conversion,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0)
    {
        switch (hardwareAcceleratorType)
        {
            case HardwareAcceleratorType.Intel:
                conversion.UseHardwareAcceleration(
                    nameof(HardwareAccelerator.d3d11va),
                    nameof(VideoCodec.h264),
                    "h264_qsv",
                    deviceNo);

                break;
            case HardwareAcceleratorType.NVIDIA:
                conversion.UseHardwareAcceleration(
                    HardwareAccelerator.d3d11va,
                    VideoCodec.h264,
                    VideoCodec.h264_nvenc,
                    deviceNo);

                break;
            case HardwareAcceleratorType.AMD:
                conversion.UseHardwareAcceleration(
                    nameof(HardwareAccelerator.d3d11va),
                    nameof(VideoCodec.h264),
                    "h264_amf",
                deviceNo);

                break;
            default:
                conversion.UseHardwareAcceleration(
                    nameof(HardwareAccelerator.d3d11va),
                    nameof(VideoCodec.h264),
                    "h264_qsv",
                    deviceNo);

                break;
        };
    }

    /// <summary>
    /// 輸出 IConversionResult
    /// </summary>
    /// <param name="conversionResult">IConversionResult</param>
    public static void WriteConversionResult(IConversionResult conversionResult)
    {
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

        _WMain?.WriteLog(
            message: MsgSet.GetFmtStr(
                MsgSet.TemplateXabeFFmpegConversionResult,
                conversionResult.StartTime.ToString(),
                conversionResult.EndTime.ToString(),
                conversionResult.Duration.ToString(),
                conversionResult.Arguments));
    }

    /// <summary>
    /// 檢查模型檔案
    /// </summary>
    /// <param name="ggmlType">GgmlType，預設值為 GgmlType.Small</param>
    /// <param name="ggmlType">QuantizationType，預設值為 GgmlType.NoQuantization</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Task&lt;string&gt;，模型檔案的路徑</returns>
    public static async Task<string> CheckModelFile(
        GgmlType ggmlType = GgmlType.Small,
        QuantizationType quantizationType = QuantizationType.NoQuantization,
        CancellationToken cancellationToken = default)
    {
        string modelFilePath = Path.Combine(
            VariableSet.ModelsFolderPath,
            WhisperUtil.GetModelFileName(ggmlType, quantizationType)),
            modelFileName = Path.GetFileName(modelFilePath);

        try
        {
            // 判斷模型檔案是否存在。
            if (!File.Exists(modelFilePath))
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperModelIsNotExists,
                        modelFileName));

                using Stream stream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(
                    ggmlType,
                    quantizationType,
                    cancellationToken);
                using FileStream fileStream = File.OpenWrite(modelFilePath);

                await stream.CopyToAsync(fileStream, cancellationToken).ContinueWith(task =>
                {
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperModelIsDownloaded,
                            modelFileName));
                }, cancellationToken);
            }
            else
            {
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperModelIsFound,
                        modelFileName));
            }
        }
        catch (OperationCanceledException)
        {
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperTranscribeCanceled,
                    modelFileName));
        }
        catch (Exception ex)
        {
            modelFilePath = string.Empty;

            _WMain?.ShowMsgBox(ex.GetExceptionMessage());
        }

        return modelFilePath;
    }

    private static void Conversion_OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.TemplateXabeFFmpegOnDataReceived,
                    e.Data));
        }
    }

    private static void Conversion_OnProgress(object sender, ConversionProgressEventArgs args)
    {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            int percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);

            if (_PBProgress != null)
            {
                _PBProgress.Value = percent;
                _PBProgress.ToolTip = $"{percent}%";
            }

            if (_LOperation != null)
            {
                string stringValue = MsgSet.GetFmtStr(
                    MsgSet.TemplateXabeFFmpegOnProgress,
                    args.ProcessId.ToString(),
                    args.Duration.ToString(),
                    args.TotalLength.ToString(),
                    percent.ToString());

                // 避免字串太長，造成顯示問題。
                string reducedStringValue = stringValue;

                StringInfo siReducedStringValue = new(reducedStringValue);

                int limitLength = Properties.Settings.Default.LOperationLimitLength;

                if (siReducedStringValue.LengthInTextElements > limitLength)
                {
                    reducedStringValue = $"{siReducedStringValue.SubstringByTextElements(0, limitLength)}{MsgSet.Ellipses}";
                }

                _LOperation.Content = reducedStringValue;
                _LOperation.ToolTip = stringValue;
            }
        }));
    }
}