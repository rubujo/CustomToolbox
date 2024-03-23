using Application = System.Windows.Application;
using ConsoleTableExt;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Utils;
using static CustomToolbox.Common.Sets.EnumSet;
using Humanizer;
using Label = System.Windows.Controls.Label;
using Microsoft.Playwright;
using OpenCCNET;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Serilog.Events;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Threading;
using Whisper.net;
using Whisper.net.Ggml;
using Whisper.net.Wave;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace CustomToolbox.Common.Sets;

/// <summary>
/// 作業組
/// </summary>
public class OperationSet
{
    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

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
        _LOperation = _WMain.LOperation;
        _PBProgress = _WMain.PBProgress;
    }

    /// <summary>
    /// 執行獲取短片資訊
    /// </summary>
    /// <param name="url">字串，影片的網址</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoFetchClipInfo(string url, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            YoutubeDL ytdl = ExternalProgram.GetYoutubeDL();

            RunResult<VideoData> result = await ytdl.RunVideoDataFetch(
                url: url,
                ct: ct,
                overrideOptions: ExternalProgram.GetConfiguredOptionSet());

            if (result.Success)
            {
                _WMain?.WriteLog(MsgSet.MsgClipInfoFetchSuceed);

                VideoData videoData = result.Data;

                string fmtResult = $"{Environment.NewLine}" +
                    $"{MsgSet.VideoDataChannel}{videoData.Channel}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataUploader}{videoData.Uploader}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataUploadDate}{videoData.UploadDate}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataID}{videoData.ID}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataTitle}{videoData.Title}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataViewCount}{videoData.ViewCount}{Environment.NewLine}" +
                    $"{MsgSet.VideoDataDescription}{Environment.NewLine}" +
                    // 替換行字元至 Environment.NewLine。
                    $"{VariableSet.RegexAscii.Replace(videoData.Description, Environment.NewLine)}" +
                    Environment.NewLine +
                    $"{MsgSet.VideoDataFormats}{Environment.NewLine}";

                List<List<object>> tableFormats = [];

                if (videoData.Formats.Length > 0)
                {
                    foreach (FormatData formatData in videoData.Formats)
                    {
                        string frameRate = string.Empty,
                            videoBitrate = string.Empty,
                            audioBitrate = string.Empty,
                            audioSamplingRate = string.Empty,
                            audioChannels = string.Empty,
                            bitrate = string.Empty,
                            fileSize = string.Empty;

                        if (formatData.FrameRate != null)
                        {
                            double fr = Math.Round(
                               formatData.FrameRate.Value,
                               MidpointRounding.AwayFromZero);

                            if (fr > 0.0f)
                            {
                                frameRate = fr.ToString();
                            }
                        }

                        if (formatData.VideoBitrate != null)
                        {
                            double vbr = Math.Round(
                                formatData.VideoBitrate.Value,
                                MidpointRounding.AwayFromZero);

                            if (vbr > 0.0d)
                            {
                                videoBitrate = $"{vbr} kbps";
                            }
                        }

                        if (formatData.AudioBitrate != null)
                        {
                            double abr = Math.Round(
                                formatData.AudioBitrate.Value,
                                MidpointRounding.AwayFromZero);

                            if (abr > 0.0d)
                            {
                                audioBitrate = $"{abr} kbps";
                            }
                        }

                        if (formatData.AudioSamplingRate != null)
                        {
                            double asr = Math.Round(
                                formatData.AudioSamplingRate.Value,
                                MidpointRounding.AwayFromZero) / 1000;

                            if (asr > 0.0d)
                            {
                                audioSamplingRate = $"{asr} kHz";
                            }
                        }

                        if (formatData.AudioChannels != null)
                        {
                            audioChannels = $"{formatData.AudioChannels} ch";
                        }

                        if (formatData.Bitrate != null)
                        {
                            double br = Math.Round(
                                formatData.Bitrate.Value,
                                MidpointRounding.AwayFromZero);

                            if (br > 0.0d)
                            {
                                bitrate = $"{br} kpbs";
                            }
                        }

                        if (formatData.FileSize != null)
                        {
                            fileSize = formatData.FileSize.Value.Bytes().ToString();
                        }
                        else
                        {
                            if (formatData.ApproximateFileSize != null)
                            {
                                fileSize = $"~{formatData.ApproximateFileSize.Value.Bytes()}";
                            }
                        }

                        tableFormats.Add(
                        [
                            formatData.Format,
                            formatData.Extension,
                            frameRate,
                            audioChannels,
                            fileSize,
                            bitrate,
                            formatData.Protocol,
                            formatData.VideoCodec,
                            videoBitrate,
                            formatData.AudioCodec,
                            audioBitrate,
                            audioSamplingRate,
                            formatData.ContainerFormat,
                            formatData.DynamicRange
                        ]);
                    }

                    // 因為排版問題，此處字串不提供 i18n 化，強制使用英文。
                    fmtResult += ConsoleTableBuilder
                        .From(tableFormats)
                        .WithColumn(new List<string>
                        {
                            "Format",
                            "Extension",
                            "Frame rate",
                            "Audio channels",
                            "File size",
                            "Bitrate",
                            "Protocol",
                            "Video codec",
                            "Video bitrate",
                            "Audio codec",
                            "Audio bitrate",
                            "Audio sampling rate",
                            "Container format",
                            "Dynamic range",
                        })
                        .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                        .Export()
                        .ToString();
                }

                if (Properties.Settings.Default.OpenCCS2TWP)
                {
                    fmtResult = ZhConverter.HansToTW(fmtResult, true);
                }

                _WMain?.WriteLog(fmtResult);
            }
            else
            {
                _WMain?.WriteLog(MsgSet.MsgClipInfoFetchFailed);

                string[] errors = result.ErrorOutput;
                string errMsg = string.Join(Environment.NewLine, errors);

                _WMain?.WriteLog(errMsg);
            }
        }
        catch (Exception ex)
        {
            _WMain?.WriteLog(ex.Message);
        }
    }

    /// <summary>
    /// 執行下載短片
    /// </summary>
    /// <param name="clipData">ClipData</param>
    /// <param name="isFullDownloadFirst">布林值，是否先下載完整短片，預設值為 false</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isDeleteSourceFile">布林值，是否刪除來源檔案，預設為 false</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoDownloadClip(
        ClipData clipData,
        bool isFullDownloadFirst = false,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isDeleteSourceFile = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            // 先處裡檔案名稱。
            string fileName = string.IsNullOrEmpty(clipData.Name) ?
                string.Empty :
                string.Join(
                    "_",
                    $"{clipData.No}.{clipData.Name}".Split(Path.GetInvalidFileNameChars()) ??
                    []);

            YoutubeDL ytdl = ExternalProgram.GetYoutubeDL();

            OptionSet configuredOptionSet = ExternalProgram.GetConfiguredOptionSet(
                    startSeconds: clipData.StartTime.TotalSeconds,
                    endSeconds: clipData.EndTime.TotalSeconds,
                    fileName: fileName,
                    isAudioOnly: clipData.IsAudioOnly,
                    isFullDownloadFirst: isFullDownloadFirst);

            RunResult<string> runResult = await ytdl.RunVideoDownload(
                url: clipData.VideoUrlOrID,
                mergeFormat: DownloadMergeFormat.Unspecified,
                recodeFormat: VideoRecodeFormat.None,
                progress: GetDownloadProgress(),
                output: GetOutputProgress(),
                ct: ct,
                overrideOptions: configuredOptionSet);

            if (runResult.Success)
            {
                ResetControls();

                _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadSucceed);
                _WMain?.WriteLog(runResult.Data);

                // 計算間隔秒數。
                double durationSeconds = clipData.EndTime.TotalSeconds -
                    clipData.StartTime.TotalSeconds;

                // 判斷是否為下載完整短片。
                if (isFullDownloadFirst)
                {
                    // 當 durationSeconds 的值大於 0 時，
                    // 需要再透過 FFmpeg 來分割短片檔案。
                    if (durationSeconds > 0)
                    {
                        await DoFFmpegTask(
                            runResult: runResult,
                            fileName: fileName,
                            clipData: clipData,
                            isFullDownloadFirst: isFullDownloadFirst,
                            useHardwareAcceleration: useHardwareAcceleration,
                            hardwareAcceleratorType: hardwareAcceleratorType,
                            deviceNo: deviceNo,
                            isDeleteSourceFile: isDeleteSourceFile,
                            ct: ct);
                    }
                }
                else
                {
                    // 當 durationSeconds 的值大於 0 時，
                    // 需要再透過 FFmpeg 來處理短片檔案。
                    if (durationSeconds > 0)
                    {
                        await DoFFmpegTask(
                            runResult: runResult,
                            fileName: fileName,
                            clipData: clipData,
                            isFullDownloadFirst: isFullDownloadFirst,
                            useHardwareAcceleration: useHardwareAcceleration,
                            hardwareAcceleratorType: hardwareAcceleratorType,
                            deviceNo: deviceNo,
                            isDeleteSourceFile: isDeleteSourceFile,
                            ct: ct);
                    }
                }
            }
            else
            {
                ResetControls();

                _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadFailure);

                if (runResult.ErrorOutput.Length != 0)
                {
                    string errMsg = string.Join(
                        Environment.NewLine,
                        runResult.ErrorOutput);

                    _WMain?.WriteLog(errMsg);
                }
            }
        }
        catch (Exception ex)
        {
            ResetControls();

            _WMain?.WriteLog(ex.Message);
        }
    }

    /// <summary>
    /// 執行下載同一網址的短片（先下載完整短片）
    /// </summary>
    /// <param name="control">DataGrid</param>
    /// <param name="clipData">ClipData</param>
    /// <param name="clipDatas">List&lt;ClipData&gt;</param>
    /// <param name="isFullDownloadFirst">布林值，是否先下載完整短片，預設值為 false</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isDeleteSourceFile">布林值，是否刪除來源檔案，預設為 false</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoDownloadClips(
        DataGrid control,
        ClipData clipData,
        List<ClipData> clipDatas,
        bool isFullDownloadFirst = false,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isDeleteSourceFile = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            // 先處裡檔案名稱。
            string fileName = string.IsNullOrEmpty(clipData.Name) ?
                string.Empty :
                string.Join(
                    "_",
                    $"{clipData.No}.{clipData.Name}".Split(Path.GetInvalidFileNameChars()) ??
                    []);

            YoutubeDL ytdl = ExternalProgram.GetYoutubeDL();

            OptionSet configuredOptionSet = ExternalProgram.GetConfiguredOptionSet(
                    startSeconds: clipData.StartTime.TotalSeconds,
                    endSeconds: clipData.EndTime.TotalSeconds,
                    fileName: fileName,
                    isAudioOnly: clipData.IsAudioOnly,
                    isFullDownloadFirst: isFullDownloadFirst);

            RunResult<string> runResult = await ytdl.RunVideoDownload(
                url: clipData.VideoUrlOrID,
                mergeFormat: DownloadMergeFormat.Unspecified,
                recodeFormat: VideoRecodeFormat.None,
                progress: GetDownloadProgress(),
                output: GetOutputProgress(),
                ct: ct,
                overrideOptions: configuredOptionSet);

            if (runResult.Success)
            {
                ResetControls();

                _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadSucceed);
                _WMain?.WriteLog(runResult.Data);

                foreach (ClipData childClipData in clipDatas)
                {
                    ct.ThrowIfCancellationRequested();

                    // 設定 DataGrid 選擇的項目來當作進度指示。
                    control.SelectedItem = childClipData;

                    // 再次處裡檔案名稱。
                    fileName = string.IsNullOrEmpty(childClipData.Name) ?
                        string.Empty :
                        string.Join(
                            "_",
                            $"{childClipData.No}.{childClipData.Name}".Split(Path.GetInvalidFileNameChars()) ??
                            []);

                    // 計算間隔秒數。
                    double durationSeconds = childClipData.EndTime.TotalSeconds -
                        childClipData.StartTime.TotalSeconds;

                    // 判斷是否為下載完整短片。
                    if (isFullDownloadFirst)
                    {
                        // 當 durationSeconds 的值大於 0 時，
                        // 需要再透過 FFmpeg 來分割短片檔案。
                        if (durationSeconds > 0)
                        {
                            await DoFFmpegTask(
                                runResult: runResult,
                                fileName: fileName,
                                clipData: childClipData,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                // 不在此處刪除來源檔案。
                                isDeleteSourceFile: false,
                                ct: ct);
                        }
                    }
                    else
                    {
                        // 當 durationSeconds 的值大於 0 時，
                        // 需要再透過 FFmpeg 來處理短片檔案。
                        if (durationSeconds > 0)
                        {
                            await DoFFmpegTask(
                                runResult: runResult,
                                fileName: fileName,
                                clipData: childClipData,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                // 不在此處刪除來源檔案。
                                isDeleteSourceFile: false,
                                ct: ct);
                        }
                    }
                }

                // 清除 DataGrid 已選擇的項目。
                control.SelectedItem = null;

                // 最後才判斷是否要刪除來源檔案。
                if (isDeleteSourceFile)
                {
                    if (File.Exists(runResult.Data))
                    {
                        File.Delete(runResult.Data);
                    }
                }
            }
            else
            {
                ResetControls();

                _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadFailure);

                if (runResult.ErrorOutput.Length != 0)
                {
                    string errMsg = string.Join(
                        Environment.NewLine,
                        runResult.ErrorOutput);

                    _WMain?.WriteLog(errMsg);
                }
            }
        }
        catch (Exception ex)
        {
            ResetControls();

            _WMain?.WriteLog(ex.Message);
        }
    }

    /// <summary>
    /// 執行下載同一網址的短片
    /// </summary>
    /// <param name="control">DataGrid</param>
    /// <param name="clipDatas">List&lt;ClipData&gt;</param>
    /// <param name="isFullDownloadFirst">布林值，是否先下載完整短片，預設值為 false</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isDeleteSourceFile">布林值，是否刪除來源檔案，預設為 false</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoDownloadClips(
        DataGrid control,
        List<ClipData> clipDatas,
        bool isFullDownloadFirst = false,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isDeleteSourceFile = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            foreach (ClipData clipData in clipDatas)
            {
                ct.ThrowIfCancellationRequested();

                // 設定 DataGrid 選擇的項目來當作進度指示。
                control.SelectedItem = clipData;

                // 先處裡檔案名稱。
                string fileName = string.IsNullOrEmpty(clipData.Name) ?
                    string.Empty :
                    string.Join(
                        "_",
                        $"{clipData.No}.{clipData.Name}".Split(Path.GetInvalidFileNameChars()) ??
                        []);

                YoutubeDL ytdl = ExternalProgram.GetYoutubeDL();

                OptionSet configuredOptionSet = ExternalProgram.GetConfiguredOptionSet(
                        startSeconds: clipData.StartTime.TotalSeconds,
                        endSeconds: clipData.EndTime.TotalSeconds,
                        fileName: fileName,
                        isAudioOnly: clipData.IsAudioOnly,
                        isFullDownloadFirst: isFullDownloadFirst);

                RunResult<string> runResult = await ytdl.RunVideoDownload(
                    url: clipData.VideoUrlOrID,
                    mergeFormat: DownloadMergeFormat.Unspecified,
                    recodeFormat: VideoRecodeFormat.None,
                    progress: GetDownloadProgress(),
                    output: GetOutputProgress(),
                    ct: ct,
                    overrideOptions: configuredOptionSet);

                if (runResult.Success)
                {
                    ResetControls();

                    _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadSucceed);
                    _WMain?.WriteLog(runResult.Data);

                    // 計算間隔秒數。
                    double durationSeconds = clipData.EndTime.TotalSeconds -
                        clipData.StartTime.TotalSeconds;

                    // 判斷是否為下載完整短片。
                    if (isFullDownloadFirst)
                    {
                        // 當 durationSeconds 的值大於 0 時，
                        // 需要再透過 FFmpeg 來分割短片檔案。
                        if (durationSeconds > 0)
                        {
                            await DoFFmpegTask(
                                runResult: runResult,
                                fileName: fileName,
                                clipData: clipData,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                isDeleteSourceFile: isDeleteSourceFile,
                                ct: ct);
                        }
                    }
                    else
                    {
                        // 當 durationSeconds 的值大於 0 時，
                        // 需要再透過 FFmpeg 來處理短片檔案。
                        if (durationSeconds > 0)
                        {
                            await DoFFmpegTask(
                                runResult: runResult,
                                fileName: fileName,
                                clipData: clipData,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                isDeleteSourceFile: isDeleteSourceFile,
                                ct: ct);
                        }
                    }
                }
                else
                {
                    ResetControls();

                    _WMain?.WriteLog(MsgSet.MsgYtDlpDownloadFailure);

                    if (runResult.ErrorOutput.Length != 0)
                    {
                        string errMsg = string.Join(
                            Environment.NewLine,
                            runResult.ErrorOutput);

                        _WMain?.WriteLog(errMsg);
                    }
                }
            }

            // 清除 DataGrid 已選擇的項目。
            control.SelectedItem = null;
        }
        catch (Exception ex)
        {
            ResetControls();

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 執行 FFmpeg 作業
    /// </summary>
    /// <param name="runResult"></param>
    /// <param name="fileName"></param>
    /// <param name="clipData">ClipData</param>
    /// <param name="isFullDownloadFirst">布林值，是否先下載完整短片，預設值為 false</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isDeleteSourceFile">布林值，是否刪除來源檔案，預設為 false</param>
    /// <param name="ct">CancellationToken</param>
    public static async Task DoFFmpegTask(
        RunResult<string> runResult,
        string fileName,
        ClipData clipData,
        bool isFullDownloadFirst = false,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isDeleteSourceFile = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            // 當為下載完整短片時，不需要修正 duration。
            bool isFixDuration = !isFullDownloadFirst;

            string fileNameSuffix = isFixDuration ? "Fixed" : "Clip",
                sourceFilePath = Path.GetFullPath(runResult.Data),
                sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath),
                sourceExtName = Path.GetExtension(sourceFilePath),
                outputFileName = string.IsNullOrEmpty(fileName) ?
                    $"{sourceFileName}_{fileNameSuffix}{sourceExtName}" :
                    $"{fileName}_{fileNameSuffix}{sourceExtName}",
                newFilePathRoot = Path.Combine(VariableSet.DownloadsFolderPath, sourceFileName),
                outputFilePath = isFixDuration ?
                    Path.Combine(VariableSet.DownloadsFolderPath, outputFileName) :
                    Path.Combine(newFilePathRoot, outputFileName);

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFilePath, ct);

            IConversion conversion = ExternalProgram.GetConversion(
                   mediaInfo,
                   clipData.StartTime.TotalSeconds,
                   clipData.EndTime.TotalSeconds,
                   outputFilePath,
                   fixDuration: isFixDuration,
                   useCodecCopy: false,
                   useHardwareAcceleration,
                   hardwareAcceleratorType,
                   deviceNo,
                   clipData.IsAudioOnly);

            // 判斷是否為下載完整短片。
            if (isFullDownloadFirst)
            {
                // 移除全部的後設資料，以利進行傻瓜式 FFmpeg 操作。
                conversion.AddParameter("-map_metadata -1");
            }

            IConversionResult conversionResult = await conversion.Start(ct);

            ExternalProgram.WriteConversionResult(conversionResult);

            if (isDeleteSourceFile)
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
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
    /// 執行 FFmpeg 作業
    /// </summary>
    /// <param name="videoFilePath">字串，影片檔案的路徑</param>
    /// <param name="fileName">字串，檔案名稱</param>
    /// <param name="clipData">ClipData</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="isDeleteSourceFile">布林值，是否刪除來源檔案，預設為 false</param>
    /// <param name="ct">CancellationToken</param>
    public static async Task DoFFmpegTask(
        string videoFilePath,
        string fileName,
        ClipData clipData,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        bool isDeleteSourceFile = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            string fileNameSuffix = "Clip",
                sourceFilePath = Path.GetFullPath(videoFilePath),
                sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath),
                sourceExtName = Path.GetExtension(sourceFilePath),
                outputFileName = string.IsNullOrEmpty(fileName) ?
                    $"{sourceFileName}_{fileNameSuffix}{sourceExtName}" :
                    $"{fileName}_{fileNameSuffix}{sourceExtName}",
                newFilePathRoot = Path.Combine(VariableSet.DownloadsFolderPath, sourceFileName),
                outputFilePath = Path.Combine(newFilePathRoot, outputFileName);

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(sourceFilePath, ct);

            IConversion conversion = ExternalProgram.GetConversion(
                   mediaInfo,
                   clipData.StartTime.TotalSeconds,
                   clipData.EndTime.TotalSeconds,
                   outputFilePath,
                   fixDuration: false,
                   useCodecCopy: false,
                   useHardwareAcceleration,
                   hardwareAcceleratorType,
                   deviceNo,
                   clipData.IsAudioOnly);

            // 移除全部的後設資料，以利進行傻瓜式 FFmpeg 操作。
            conversion.AddParameter("-map_metadata -1");

            IConversionResult conversionResult = await conversion.Start(ct);

            ExternalProgram.WriteConversionResult(conversionResult);

            if (isDeleteSourceFile)
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Delete(sourceFilePath);
                }
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
    /// 執行燒錄字幕檔
    /// </summary>
    /// <param name="videoFilePath">字串，視訊檔案的路徑</param>
    /// <param name="subtitleFilePath">字串，字幕檔案的路徑</param>
    /// <param name="outputPath">字串，輸出檔案的路徑</param>
    /// <param name="encoding">字串，字幕檔案的文字編碼，預設值為 "UTF-8"</param>
    /// <param name="applyFontSetting">布林值，套用字型設定，預設值為 false</param>
    /// <param name="fontName">字串，字型名稱，預設值為 null</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">EnumSet.HardwareAcceleratorType，硬體的類型，預設是 EnumSet.HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoBurnInSubtitle(
        string videoFilePath,
        string subtitleFilePath,
        string outputPath,
        string encoding = "UTF-8",
        bool applyFontSetting = false,
        string? fontName = null,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            string videoExtName = Path.GetExtension(videoFilePath);

            // 取得影片的資訊。
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoFilePath, ct);

            switch (videoExtName)
            {
                case ".mp4":
                    {
                        IConversion conversion = ExternalProgram.GetBurnInSubtitleConversion(
                            mediaInfo,
                            subtitleFilePath,
                            outputPath,
                            encoding,
                            applyFontSetting,
                            fontName,
                            useHardwareAcceleration,
                            hardwareAcceleratorType,
                            deviceNo);

                        IConversionResult conversionResult = await conversion.Start(ct);

                        ExternalProgram.WriteConversionResult(conversionResult);
                    }

                    break;
                case ".mkv":
                    {
                        // 取得字幕檔的資訊。
                        IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(subtitleFilePath, ct);

                        IConversion conversion = ExternalProgram.GetAddSubtitleStreamConversion(
                            mediaInfo,
                            subtitleInfo,
                            outputPath,
                            useHardwareAcceleration,
                            hardwareAcceleratorType,
                            deviceNo);

                        IConversionResult conversionResult = await conversion.Start(ct);

                        ExternalProgram.WriteConversionResult(conversionResult);
                    }

                    break;
                default:
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgErrorOccured,
                            MsgSet.MsgSelectedVideoNonSupported),
                        logEventLevel: LogEventLevel.Error);

                    break;
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
    /// 執行分割影片
    /// </summary>
    /// <param name="control">DataGrid</param>
    /// <param name="clipDatas">List&lt;ClipData&gt;</param>
    /// <param name="videoFilePath">字串，影片檔案的路徑</param>
    /// <param name="useHardwareAcceleration">布林值，是否使用硬體加速解編碼，預設值為 false</param>
    /// <param name="hardwareAcceleratorType">HardwareAcceleratorType，硬體加速的類型，預設值為 HardwareAcceleratorType.Intel</param>
    /// <param name="deviceNo">數值，GPU 裝置的 ID 值，預設為 0</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoSplitVideo(
        DataGrid control,
        List<ClipData> clipDatas,
        string videoFilePath,
        bool useHardwareAcceleration = false,
        HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel,
        int deviceNo = 0,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            foreach (ClipData clipData in clipDatas)
            {
                ct.ThrowIfCancellationRequested();

                // 設定 DataGrid 選擇的項目來當作進度指示。
                control.SelectedItem = clipData;

                // 先處裡檔案名稱。
                string fileName = string.IsNullOrEmpty(clipData.Name) ?
                    string.Empty :
                    string.Join(
                        "_",
                        $"{clipData.No}.{clipData.Name}".Split(Path.GetInvalidFileNameChars()) ??
                        []);

                await DoFFmpegTask(
                    videoFilePath: videoFilePath,
                    fileName: fileName,
                    clipData: clipData,
                    useHardwareAcceleration: useHardwareAcceleration,
                    hardwareAcceleratorType: hardwareAcceleratorType,
                    deviceNo: deviceNo,
                    // 此處不刪除來源檔案。
                    isDeleteSourceFile: false,
                    ct: ct);
            }

            // 清除 DataGrid 已選擇的項目。
            control.SelectedItem = null;
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
    /// 執行產生 Bilibili 指定使用者的短片清單檔案
    /// </summary>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <param name="exportJsonc">布林值，是否匯出 *.jsonc 格式，預設值為 false</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoGenerateB23ClipList(
        string mid,
        bool exportJsonc = false,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            await B23ClipUtil.GenerateClipFiles(
                listMID: [mid],
                folderPath: VariableSet.ClipListsFolderPath,
                forceChromium: false,
                maxReTryTimes: 3,
                openFolderWhenFinished: true,
                exportJsonc: exportJsonc,
                hansToTW: Properties.Settings.Default.OpenCCS2TWP,
                ct: ct);
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
    /// 執行拍攝 YouTube 訂閱者數量截圖
    /// </summary>
    /// <param name="channelId">字串，YouTube 頻道的 ID</param>
    /// <param name="inputSavedPath">字串，截圖的儲存路徑，預設值為空白</param>
    /// <param name="customSubscriberAmount">數值，自定義頻道訂閱者數量，預設值為 -1</param>
    /// <param name="useTranslate">布林值，套用中文翻譯，預設值為 false</param>
    /// <param name="useClip">布林值，將截圖裁切成正方形，預設值為 false</param>
    /// <param name="addTimestamp">布林值，加入日期戳記，預設值為 false</param>
    /// <param name="customTimestamp">字串，自定義的日期戳記，預設值為空白</param>
    /// <param name="blurBackground">布林值，模糊背景，預設值為 false</param>
    /// <param name="forceChromium">布林值，強制使用 Chromium，預設值為 false</param>
    /// <param name="isDevelopmentMode">布林值，開發模式，預設值為 false</param>
    /// <returns>Task</returns>
    public static async Task DoTakeYtscScrnshot(
        string channelId,
        string inputSavedPath = "",
        decimal customSubscriberAmount = -1,
        bool useTranslate = false,
        bool useClip = false,
        bool addTimestamp = false,
        string customTimestamp = "",
        bool blurBackground = false,
        bool forceChromium = false,
        bool isDevelopmentMode = false)
    {
        try
        {
            // 偵測 Playwright 使用的網頁瀏覽器。
            string browserChannel = await PlaywrightUtil.DetectBrowser(forceChromium);

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgYtscToolUsedBrowserChannel,
                    browserChannel));

            using IPlaywright playwright = await Playwright.CreateAsync();

            await using IBrowser browser = await playwright.Chromium.LaunchAsync(new()
            {
                Channel = browserChannel,
                Headless = !isDevelopmentMode
            });

            IPage page = await browser.NewPageAsync();

            // 瀏覽 YouTube Subscriber Counter 網站。
            await page.GotoAsync($"{UrlSet.YTSubscriberCounterUrl}{channelId}");

            // 隱藏 Header 區塊。
            await page.EvalOnSelectorAsync(
                SelectorSet.HeaderBlock,
                ExpressionSet.HideElement);

            // 判斷是否要裁切截圖。
            if (useClip)
            {
                // 取頁面上的頻道名稱文字。
                string channelName = await page.InnerTextAsync(SelectorSet.ChannelNameBlock);

                channelName = channelName.TrimEnd();

                // 判斷是否需要截斷頻道名稱。
                if (channelName.Length > VariableSet.SplitLength)
                {
                    int[] intArray = PlaywrightUtil.GetIntArray(channelName);

                    string[]? nameArray = channelName.Split(intArray);

                    if (nameArray != null)
                    {
                        if (nameArray.Length < VariableSet.ChannelNameRowLimit)
                        {
                            string newChannelName = string.Empty;

                            foreach (string str in nameArray)
                            {
                                newChannelName += $"<div>{str}</div>";
                            }

                            if (!string.IsNullOrEmpty(newChannelName))
                            {
                                // 替換文字。
                                await page.EvalOnSelectorAsync(
                                    SelectorSet.ChannelNameBlock,
                                    ExpressionSet.ChangeChannelNameBlock.Replace("{Value}", newChannelName));
                            }
                        }
                        else
                        {
                            // 當超出限制列數時，取消裁切截圖。
                            useClip = false;

                            _WMain?.WriteLog(
                                message: MsgSet.GetFmtStr(
                                    MsgSet.MsgYtscToolChNameTooLongCancelCut,
                                    VariableSet.ChannelNameRowLimit.ToString()));
                        }
                    }
                }
            }

            // 判斷是否要翻譯文字。
            if (useTranslate)
            {
                // 替換文字。
                await page.EvalOnSelectorAsync(
                    SelectorSet.PrefixText,
                    ExpressionSet.ChangePrefixText);

                // 替換文字。
                await page.EvalOnSelectorAsync(
                    SelectorSet.SuffixText,
                    ExpressionSet.ChangeSuffixText);
            }

            // 判斷是否要加入日期戳記
            if (addTimestamp)
            {
                // 當 customTimestamp 為空值時，使用今日。
                if (string.IsNullOrEmpty(customTimestamp))
                {
                    customTimestamp = DateTime.Now.ToShortDateString();
                }

                // 統一使用 "."。
                customTimestamp = customTimestamp
                    .Replace("/", ".")
                    .Replace("-", ".")
                    .Replace(" ", ".");

                // 替換文字。
                await page.EvalOnSelectorAsync(
                    useTranslate ? SelectorSet.TranslateSuffixText : SelectorSet.SuffixText,
                    ExpressionSet.ChangeSuffixText1Dot5.Replace("{Value}", customTimestamp));
            }

            // 點選對話視窗的按鈕。
            await page.ClickAsync(SelectorSet.AlertDialogButton);

            // 判斷是否要模糊背景。
            if (blurBackground)
            {
                // 點選設定按鈕。
                await page.EvalOnSelectorAsync(SelectorSet.SettingButton, ExpressionSet.ClickElement);

                // 點選模糊背景。
                await page.EvalOnSelectorAsync(SelectorSet.BlurBackgroundCheckbox, ExpressionSet.ClickElement);

                // 點選關閉按鈕。
                await page.EvalOnSelectorAsync(SelectorSet.CloseButton, ExpressionSet.ClickElement);
            }

            // 隱藏 Chart 區塊。
            await page.EvalOnSelectorAsync(
                SelectorSet.ChartBlock,
                ExpressionSet.HideElement);

            // 判斷是否有設定自定義訂閱數。
            if (customSubscriberAmount > 0)
            {
                string acutalNumber = string.Format("{0:N0}", customSubscriberAmount);

                string tempHtml = string.Empty;

                foreach (char c in acutalNumber.ToCharArray())
                {
                    tempHtml += $"{PlaywrightUtil.SetHTML(c)}";
                }

                string expression = ExpressionSet.ChangeSubscribersBlock
                    .Replace("{Value}", tempHtml);

                // 設定自定義訂閱數。
                await page.EvalOnSelectorAsync(SelectorSet.SubscribersBlock, expression);
            }
            else
            {
                // 取頁面上的訂閱者數字。
                string subscriberAmount = await page.InnerTextAsync(SelectorSet.SubscribersBlock);

                // 將字串後處理。
                // 將字串後處理。
                subscriberAmount = subscriberAmount
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Replace("\t", string.Empty)
                    .Replace(",", string.Empty);

                // 將處理過的字串轉換成 decimal。
                customSubscriberAmount = decimal.TryParse(subscriberAmount, out decimal parsedResult) ?
                    parsedResult : -1;
            }

            // 延後執行，讓對話視窗有時間關閉。
            Thread.Sleep(VariableSet.SleepMs);

            // 截圖的儲存路徑。
            string savedPath = string.IsNullOrEmpty(inputSavedPath) ?
                Path.Combine(
                    $@"C:\Users\{Environment.UserName}\Desktop",
                    $"Screenshot_{DateTime.Now:yyyyMMddHHmmss}.png") :
                inputSavedPath;

            // 當變數值相等時才判斷目的地的資料夾是否存在。
            if (savedPath == inputSavedPath)
            {
                string? folderPath = Path.GetDirectoryName(savedPath) ?? string.Empty;

                if (!string.IsNullOrEmpty(folderPath) &
                    !Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }

            // 取得副檔名。
            string extName = Path.GetExtension(savedPath);

            // 拍攝頁面的截圖。
            await page.ScreenshotAsync(new()
            {
                Path = savedPath,
                Timeout = VariableSet.ScreenshotTimeout,
                Clip = useClip ? PlaywrightUtil.GetClip(customSubscriberAmount) : null,
                Type = extName == ".png" ? ScreenshotType.Png : ScreenshotType.Jpeg
            });

            _WMain?.WriteLog(message: MsgSet.MsgYtscToolTakeScreenshotFinished);
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgYtscToolFileSaveAt,
                    savedPath));

            // 僅供測試使用。
            if (isDevelopmentMode)
            {
                Thread.Sleep(VariableSet.DevSleepMs);
            }
        }
        catch (Exception ex)
        {
            // 只有可以 Cancel 的才只輸出 ex.Message.ToString()。
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 執行轉換成 WAV 檔案
    /// </summary>
    /// <param name="inputFilePath">字串，檔案的路徑</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task&lt;string&gt;，產生的 WAV 檔案的路徑</returns>
    public static async Task<string> DoConvertToWavFile(
        string inputFilePath,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            string fileName = Path.GetFileNameWithoutExtension(inputFilePath);

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFilePath, ct);

            IEnumerable<IAudioStream> audioStreams = mediaInfo.AudioStreams;

            if (audioStreams == null)
            {
                _WMain?.WriteLog(
                    message: MsgSet.MsgSelectAValidVideoOrAudioFile,
                    logEventLevel: LogEventLevel.Warning);

                return string.Empty;
            }

            string tempFilePath = Path.Combine(
                VariableSet.TempFolderPath,
                $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}.wav");

            IConversion conversion = ExternalProgram
                .GetConvertToWavConversion(audioStreams, tempFilePath);

            IConversionResult conversionResult = await conversion.Start(ct);

            ExternalProgram.WriteConversionResult(conversionResult);

            return tempFilePath;
        }
        catch (OperationCanceledException)
        {
            _WMain?.WriteLog(message: MsgSet.MsgJobHasCanceled);
        }
        catch (Exception ex)
        {
            _WMain?.ShowMsgBox(message: ex.GetExceptionMessage());
        }

        return string.Empty;
    }

    /// <summary>
    /// 執行偵測語言
    /// <para>因為會發生 System.AccessViolationException，故 speedUp 需設為 false。</para>
    /// </summary>
    /// <param name="inputFilePath">字串，檔案的路徑</param>
    /// <param name="language">字串，語言（兩碼），預設值為 "auto"</param>
    /// <param name="enableTranslate">布林值，啟用翻譯成英文，預設值為 false</param>
    /// <param name="enableSpeedUp2x">布林值，啟用 SpeedUp2x，預設值為 false</param>
    /// <param name="speedUp">布林值，是否加速，預設值為 false</param>
    /// <param name="ggmlType">GgmlType，預設值為 GgmlType.Small</param>
    /// <param name="quantizationType">QuantizationType，預設值為 QuantizationType.NoQuantization</param>
    /// <param name="samplingStrategyType">SamplingStrategyType，預設值為 SamplingStrategyType.Default</param>
    /// <param name="beamSize">beamSize，用於 SamplingStrategyType.BeamSearch，預設值為 5</param>
    /// <param name="patience">patience，用於 SamplingStrategyType.BeamSearch，預設值為 -0.1f</param>
    /// <param name="bestOf">bestOf，用於 SamplingStrategyType.Greedy，預設值為 1</param>
    /// <param name="prompt">字串，提示詞，預設值為空白</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoDetectLanguage(
        string inputFilePath,
        string language = "auto",
        bool enableTranslate = false,
        bool enableSpeedUp2x = false,
        bool speedUp = false,
        GgmlType ggmlType = GgmlType.Small,
        QuantizationType quantizationType = QuantizationType.NoQuantization,
        SamplingStrategyType samplingStrategyType = SamplingStrategyType.Default,
        int beamSize = 5,
        float patience = -0.1f,
        int bestOf = 1,
        string prompt = "",
        CancellationToken cancellationToken = default)
    {
        Stopwatch stopWatch = new();

        stopWatch.Start();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string tempFilePath = await Task.Run(async () =>
            {
                string wavfilePath = await DoConvertToWavFile(
                        inputFilePath,
                        cancellationToken),
                    modelFilePath = await ExternalProgram.CheckModelFile(
                        ggmlType,
                        quantizationType,
                        cancellationToken);

                if (string.IsNullOrEmpty(modelFilePath))
                {
                    _WMain?.WriteLog(message: MsgSet.MsgWhisperModelFileNotFound);
                    _WMain?.WriteLog(message: MsgSet.MsgWhisperDetectLanguageCanceled);
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                            VariableSet.TempFolderPath));

                    return string.Empty;
                }

                _WMain?.WriteLog(message: MsgSet.MsgWhisperDetectLanguageStarting);
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperUsedModel,
                        ggmlType.ToString()));
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperUsedQuantizationType,
                        quantizationType.ToString()));

                using WhisperFactory whisperFactory = WhisperFactory.FromPath(modelFilePath);

                WhisperProcessorBuilder whisperProcessorBuilder = whisperFactory.CreateBuilder()
                    .WithEncoderBeginHandler(WhisperDotNet_OnEncoderBegin)
                    .WithProgressHandler(WhisperDotNet_OnProgress)
                    .WithSegmentEventHandler(WhisperDotNet_OnNewSegment);

                if (language == "auto")
                {
                    whisperProcessorBuilder.WithLanguageDetection();
                }
                else
                {
                    whisperProcessorBuilder.WithLanguage(language);
                }

                if (enableTranslate)
                {
                    whisperProcessorBuilder.WithTranslate();
                }

                if (enableSpeedUp2x)
                {
                    whisperProcessorBuilder.WithSpeedUp2x();
                }

                if (!string.IsNullOrEmpty(prompt))
                {
                    whisperProcessorBuilder.WithPrompt(prompt);
                }

                WhisperProcessor whisperProcessor = WhisperUtil.GetWhisperProcessor(
                    whisperProcessorBuilder: whisperProcessorBuilder,
                    samplingStrategyType: samplingStrategyType,
                    beamSize: beamSize,
                    patience: patience,
                    bestOf: bestOf);

                using FileStream fileStream = File.OpenRead(wavfilePath);

                WaveParser waveParser = new(fileStream);

                bool isTaskCanceled = false;

                try
                {
                    float[] avgSamples = await waveParser.GetAvgSamplesAsync(cancellationToken);

                    (string? detectedLanguage, float? probability) = whisperProcessor
                        .DetectLanguageWithProbability(samples: avgSamples, speedUp: speedUp);

                    string rawResult = string.IsNullOrEmpty(detectedLanguage) ?
                            MsgSet.MsgWhisperDetectLanguageFailed :
                            MsgSet.GetFmtStr(
                                MsgSet.TemplateWhipserDetectLaunguageResult,
                                detectedLanguage,
                                $"{probability:P}"),
                        resultMessage = MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperDetectLanguageResult,
                            rawResult);

                    _WMain?.WriteLog(message: resultMessage);

                    _WMain?.ShowMsgBox(message: resultMessage);
                }
                catch (OperationCanceledException)
                {
                    isTaskCanceled = true;

                    stopWatch.Stop();

                    _WMain?.WriteLog(message: MsgSet.MsgWhisperDetectLanguageCanceled);
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperTotalElapsed,
                            stopWatch.Elapsed.ToFFmpeg()));
                }

                await whisperProcessor.DisposeAsync();

                if (!isTaskCanceled)
                {
                    stopWatch.Stop();

                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperTotalElapsed,
                            stopWatch.Elapsed.ToFFmpeg()));
                }

                return wavfilePath;
            }, cancellationToken);

            if (!string.IsNullOrEmpty(tempFilePath) &&
                File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);

                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperTempFileDeleted,
                        tempFilePath));
            }

            ResetControls();
        }
        catch (OperationCanceledException)
        {
            stopWatch.Stop();

            _WMain?.WriteLog(message: MsgSet.MsgWhisperDetectLanguageCanceled);
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperTotalElapsed,
                    stopWatch.Elapsed.ToFFmpeg()));
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                    VariableSet.TempFolderPath));
        }
        catch (Exception ex)
        {
            ResetControls();

            stopWatch.Stop();

            _WMain?.WriteLog(message: MsgSet.MsgWhisperDetectLanguageCanceled);
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperTotalElapsed,
                    stopWatch.Elapsed.ToFFmpeg()));
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                    VariableSet.TempFolderPath));

            _WMain?.ShowMsgBox(message: ex.GetExceptionMessage());
        }
    }

    /// <summary>
    /// 執行轉譯
    /// </summary>
    /// <param name="inputFilePath">字串，檔案的路徑</param>
    /// <param name="language">字串，語言（兩碼），預設值為 "auto"</param>
    /// <param name="enableTranslate">布林值，啟用翻譯成英文，預設值為 false</param>
    /// <param name="enableSpeedUp2x">布林值，啟用 SpeedUp2x，預設值為 false</param>
    /// <param name="exportWebVtt">布林值，匯出 WebVTT 格式，預設值為 false</param>
    /// <param name="ggmlType">GgmlType，預設值為 GgmlType.Small</param>
    /// <param name="quantizationType">QuantizationType，預設值為 QuantizationType.NoQuantization</param>
    /// <param name="samplingStrategyType">SamplingStrategyType，預設值為 SamplingStrategyType.Default</param>
    /// <param name="beamSize">beamSize，用於 SamplingStrategyType.BeamSearch，預設值為 5</param>
    /// <param name="patience">patience，用於 SamplingStrategyType.BeamSearch，預設值為 -0.1f</param>
    /// <param name="bestOf">bestOf，用於 SamplingStrategyType.Greedy，預設值為 1</param>
    /// <param name="prompt">字串，提示詞，預設值為空白</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task DoTranscribe(
        string inputFilePath,
        string language = "auto",
        bool enableTranslate = false,
        bool enableSpeedUp2x = false,
        bool exportWebVtt = false,
        GgmlType ggmlType = GgmlType.Small,
        QuantizationType quantizationType = QuantizationType.NoQuantization,
        SamplingStrategyType samplingStrategyType = SamplingStrategyType.Default,
        int beamSize = 5,
        float patience = -0.1f,
        int bestOf = 1,
        string prompt = "",
        CancellationToken cancellationToken = default)
    {
        Stopwatch stopWatch = new();

        stopWatch.Start();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string tempFilePath = await Task.Run(async () =>
            {
                List<SegmentData> segmentDataSet = [];

                string wavfilePath = await DoConvertToWavFile(
                        inputFilePath,
                        cancellationToken),
                    modelFilePath = await ExternalProgram.CheckModelFile(
                        ggmlType,
                        quantizationType,
                        cancellationToken);

                if (string.IsNullOrEmpty(modelFilePath))
                {
                    _WMain?.WriteLog(message: MsgSet.MsgWhisperModelFileNotFound);
                    _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeCanceled);
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                            VariableSet.TempFolderPath));

                    return string.Empty;
                }

                _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeStarting);
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperUsedModel,
                        ggmlType.ToString()));
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperUsedQuantizationType,
                        quantizationType.ToString()));
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperTranscribeUsedLanguage,
                        language));
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperTranscribeUsedSamplingStrategyType,
                        samplingStrategyType.ToString()));
                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperTranscribeEnableOpenCCS2TWP,
                        Properties.Settings.Default.OpenCCS2TWP ? MsgSet.Yes : MsgSet.No));

                using WhisperFactory whisperFactory = WhisperFactory.FromPath(modelFilePath);

                WhisperProcessorBuilder whisperProcessorBuilder = whisperFactory.CreateBuilder()
                    .WithEncoderBeginHandler(WhisperDotNet_OnEncoderBegin)
                    .WithProgressHandler(WhisperDotNet_OnProgress)
                    .WithSegmentEventHandler(WhisperDotNet_OnNewSegment)
                    .WithProbabilities();

                if (language == "auto")
                {
                    whisperProcessorBuilder.WithLanguageDetection();
                }
                else
                {
                    whisperProcessorBuilder.WithLanguage(language);
                }

                if (enableTranslate)
                {
                    whisperProcessorBuilder.WithTranslate();
                }

                if (enableSpeedUp2x)
                {
                    whisperProcessorBuilder.WithSpeedUp2x();
                }

                if (!string.IsNullOrEmpty(prompt))
                {
                    whisperProcessorBuilder.WithPrompt(prompt);
                }

                WhisperProcessor whisperProcessor = WhisperUtil.GetWhisperProcessor(
                    whisperProcessorBuilder: whisperProcessorBuilder,
                    samplingStrategyType: samplingStrategyType,
                    beamSize: beamSize,
                    patience: patience,
                    bestOf: bestOf);

                using FileStream fileStream = File.OpenRead(wavfilePath);

                _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeContent);

                bool isTaskCanceled = false;

                try
                {
                    await foreach (SegmentData segmentData in whisperProcessor
                        .ProcessAsync(fileStream, cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        segmentDataSet.Add(segmentData);
                    }
                }
                catch (OperationCanceledException)
                {
                    isTaskCanceled = true;

                    stopWatch.Stop();

                    _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeCanceled);
                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperTotalElapsed,
                            stopWatch.Elapsed.ToFFmpeg()));
                }

                await whisperProcessor.DisposeAsync();

                if (!isTaskCanceled)
                {
                    stopWatch.Stop();

                    _WMain?.WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgWhisperTotalElapsed,
                            stopWatch.Elapsed.ToFFmpeg()));
                    _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeFinished);

                    // 建立字幕檔。
                    string subtitleFilePath = DoCreateSubtitleFile(
                            segmentDataSet,
                            inputFilePath,
                            exportWebVtt),
                        subtitleFileName = Path.GetFileName(subtitleFilePath),
                        subtitleFileFolder = Path.GetFullPath(subtitleFilePath)
                            .Replace(subtitleFileName, string.Empty);

                    // 開啟資料夾。
                    CustomFunction.OpenFolder(subtitleFileFolder);

                    _WMain?.ShowMsgBox(message: MsgSet.MsgWhisperTranscribeFinished);
                }

                return wavfilePath;
            }, cancellationToken);

            if (!string.IsNullOrEmpty(tempFilePath) &&
                File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);

                _WMain?.WriteLog(
                    message: MsgSet.GetFmtStr(
                        MsgSet.MsgWhisperTempFileDeleted,
                        tempFilePath));
            }

            ResetControls();
        }
        catch (OperationCanceledException)
        {
            stopWatch.Stop();

            _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeCanceled);
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperTotalElapsed,
                    stopWatch.Elapsed.ToFFmpeg()));
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                    VariableSet.TempFolderPath));
        }
        catch (Exception ex)
        {
            ResetControls();

            stopWatch.Stop();

            _WMain?.WriteLog(message: MsgSet.MsgWhisperTranscribeCanceled);
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperTotalElapsed,
                    stopWatch.Elapsed.ToFFmpeg()));
            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperRemoveTempFileByYourSelf,
                    VariableSet.TempFolderPath));

            _WMain?.ShowMsgBox(ex.GetExceptionMessage());
        }
    }

    /// <summary>
    /// 執行建立字幕檔案
    /// </summary>
    /// <param name="segmentDataSet">List&lt;SegmentData&gt;</param>
    /// <param name="inputFilePath">字串，檔案的路徑</param>
    /// <param name="exportWebVTT">布林值，匯出 WebVTT 格式，預設值為 false</param>
    /// <returns>字串，字幕檔案的路徑</returns>
    public static string DoCreateSubtitleFile(
        List<SegmentData> segmentDataSet,
        string inputFilePath,
        bool exportWebVTT)
    {
        string filePath1 = Path.ChangeExtension(inputFilePath, ".srt");

        _WMain?.WriteLog(
            message: MsgSet.GetFmtStr(
                MsgSet.MsgWhisperStartToCreateSubtitleFile,
                "SubRip Text"));

        using StreamWriter streamWriter1 = File.CreateText(filePath1);

        for (int i = 0; i < segmentDataSet.Count; i++)
        {
            streamWriter1.WriteLine(i + 1);

            SegmentData segmentData = segmentDataSet[i];

            string startTime = WhisperUtil.PrintTimeWithComma(segmentData.Start),
                endTime = WhisperUtil.PrintTimeWithComma(segmentData.End);

            streamWriter1.WriteLine("{0} --> {1}", startTime, endTime);
            streamWriter1.WriteLine(WhisperUtil.GetSegmentDataText(segmentData));
            streamWriter1.WriteLine();
        }

        _WMain?.WriteLog(
            message: MsgSet.GetFmtStr(
                MsgSet.MsgWhisperSubtitleFileCreated,
                "SubRip Text",
                filePath1));

        #region WebVTT

        if (exportWebVTT)
        {
            string filePath2 = Path.ChangeExtension(inputFilePath, ".vtt");

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperStartToCreateSubtitleFile,
                    "WebVTT"));

            using StreamWriter streamWriter2 = File.CreateText(filePath2);

            streamWriter2.WriteLine("WEBVTT ");
            streamWriter2.WriteLine();

            for (int i = 0; i < segmentDataSet.Count; i++)
            {
                streamWriter2.WriteLine(i + 1);

                SegmentData segmentData = segmentDataSet[i];

                string startTime = WhisperUtil.PrintTime(segmentData.Start),
                    endTime = WhisperUtil.PrintTime(segmentData.End);

                streamWriter2.WriteLine("{0} --> {1}", startTime, endTime);
                streamWriter2.WriteLine(WhisperUtil.GetSegmentDataText(segmentData));
                streamWriter2.WriteLine();
            }

            _WMain?.WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgWhisperSubtitleFileCreated,
                    "WebVTT",
                    filePath2));
        }

        #endregion

        return filePath1;
    }

    /// <summary>
    /// 取得 Progress&lt;DownloadProgress&gt;
    /// </summary>
    /// <returns>Progress&lt;DownloadProgress&gt;</returns>
    private static Progress<DownloadProgress> GetDownloadProgress()
    {
        return new Progress<DownloadProgress>(downloadProgress =>
        {
            Application.Current.Dispatcher.BeginInvoke(
                method: new Action(() =>
                {
                    if (_LOperation == null || _PBProgress == null)
                    {
                        return;
                    }

                    double currentPercent = downloadProgress.Progress * 100;

                    _PBProgress.Value = currentPercent;
                    _PBProgress.ToolTip = $"{currentPercent}%";

                    string message = $"({downloadProgress.State})";

                    message += MsgSet.GetFmtStr(
                        MsgSet.YtdlSharpVideoIndex,
                        downloadProgress.VideoIndex.ToString());

                    if (!string.IsNullOrEmpty(downloadProgress.DownloadSpeed))
                    {
                        message += MsgSet.GetFmtStr(
                            MsgSet.YtdlSharpDownloadSpeed,
                            downloadProgress.DownloadSpeed);
                    }

                    if (!string.IsNullOrEmpty(downloadProgress.ETA))
                    {
                        message += MsgSet.GetFmtStr(
                            MsgSet.YtdlSharpETA,
                            downloadProgress.ETA);
                    }

                    if (!string.IsNullOrEmpty(downloadProgress.TotalDownloadSize))
                    {
                        message += MsgSet.GetFmtStr(
                            MsgSet.YtdlSharpTotalDownloadSize,
                            downloadProgress.TotalDownloadSize);
                    }

                    if (!string.IsNullOrEmpty(downloadProgress.Data))
                    {
                        message += MsgSet.GetFmtStr(
                            MsgSet.YtdlSharpData,
                            downloadProgress.Data);
                    }

                    // 避免字串太長，造成顯示問題。
                    string reducedMessage = message;

                    StringInfo siReducedMessage = new(reducedMessage);

                    int limitLength = Properties.Settings.Default.LOperationLimitLength;

                    if (siReducedMessage.LengthInTextElements > limitLength)
                    {
                        reducedMessage = $"{siReducedMessage.SubstringByTextElements(0, limitLength)}{MsgSet.Ellipses}";
                    }

                    _LOperation.Content = reducedMessage;
                    _LOperation.ToolTip = message;
                }),
                priority: DispatcherPriority.Background);
        });
    }

    /// <summary>
    /// 取得 Progress&lt;string&gt;
    /// </summary>
    /// <returns>Progress&lt;string&gt;</returns>
    private static Progress<string> GetOutputProgress()
    {
        string tempFrag = string.Empty,
            tempValue = string.Empty;

        return new Progress<string>(value =>
        {
            value = value.TrimEnd();

            // 手動減速機制，針對 "(frag " 開的字串進行減速。
            int starIdx = value.LastIndexOf("(frag ");

            if (starIdx > -1)
            {
                string fragPart = value[starIdx..]
                    .Replace("(frag ", string.Empty)
                    .Replace(")", string.Empty);

                if (tempFrag != fragPart)
                {
                    // 2023/12/28 因為這類型的訊息很容易造成應用程式卡死，
                    // 所以不使用 _WMain?.WriteLog() 方法直接輸出訊息。
                    //_WMain?.WriteLog(message: value);

                    if (_LOperation != null)
                    {
                        _LOperation.Content = value;
                        _LOperation.ToolTip = value;
                    }

                    tempFrag = fragPart;
                }
            }
            else
            {
                // 手動減速機制，讓前後一樣的字串不輸出。
                if (value != tempValue)
                {
                    _WMain?.WriteLog(message: value);
                }
            }

            tempValue = value;
        });
    }

    /// <summary>
    /// 重設控制項
    /// </summary>
    public static async void ResetControls()
    {
        try
        {
            await Application.Current.Dispatcher.BeginInvoke(
                method: new Action(() =>
                {
                    if (_LOperation == null || _PBProgress == null)
                    {
                        return;
                    }

                    _LOperation.Content = string.Empty;
                    _LOperation.ToolTip = string.Empty;
                    _PBProgress.Value = 0.0d;
                    _PBProgress.ToolTip = string.Empty;
                }),
                priority: DispatcherPriority.Background);
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
    /// whisper.net 的編碼器開始事件
    /// </summary>
    /// <param name="encoderBeginData">EncoderBeginData</param>
    private static bool WhisperDotNet_OnEncoderBegin(EncoderBeginData encoderBeginData)
    {
        _ = encoderBeginData;

        if (_WMain?.GlobalCTS != null &&
            _WMain?.GlobalCTS.IsCancellationRequested == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// whisper.net 的進度事件
    /// </summary>
    /// <param name="porgress">數值，進度</param>
    private static void WhisperDotNet_OnProgress(int porgress)
    {
        Application.Current.Dispatcher.BeginInvoke(
            method: new Action(() =>
            {
                if (_PBProgress != null)
                {
                    _PBProgress.Value = porgress;
                    _PBProgress.ToolTip = $"{porgress}%";
                }
            }),
            priority: DispatcherPriority.Background);
    }

    /// <summary>
    /// whisper.net 的新段事件
    /// </summary>
    /// <param name="segmentData">SegmentData</param>
    private static void WhisperDotNet_OnNewSegment(SegmentData segmentData)
    {
        string segment = $"{segmentData.Start} --> {segmentData.End}：" +
                $"[ {segmentData.Language} ({segmentData.Probability:P}) ] " +
                $"{segmentData.Text}";

        _WMain?.WriteLog(message: segment);
    }
}