using Application = System.Windows.Application;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using CustomToolbox.Common;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Models.ImportPlaylist;
using CustomToolbox.Common.Models.UpdateNotifier;
using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Utils;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using MessageBox = System.Windows.MessageBox;
using OpenCCNET;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Xabe.FFmpeg;

namespace CustomToolbox;

/// <summary>
/// WMain 的方法
/// </summary>
public partial class WMain
{
    /// <summary>
    /// 顯示訊息
    /// </summary>
    /// <param name="message">字串，訊息</param>
    /// <param name="title">字串，標題，預設值為空白</param>
    public void ShowMsgBox(
        string message,
        string title = "")
    {
        Application.Current.Dispatcher.BeginInvoke(
            method: new Action(() =>
            {
                MessageBox.Show(
                    owner: this,
                    messageBoxText: message,
                    caption: title,
                    button: MessageBoxButton.OK);
            }),
            priority: DispatcherPriority.Normal);
    }

    /// <summary>
    /// 顯示確認訊息
    /// </summary>
    /// <param name="message">字串，訊息</param>
    /// <param name="primaryAction">Action，主要按鈕的 Action</param>
    /// <param name="cancelAction">Action，取消按鈕的 Action</param>
    /// <param name="secondaryAction">Action，第二按鈕的 Action</param>
    /// <param name="title">字串，標題，預設值為空白</param>
    public void ShowConfirmMsgBox(
        string message,
        Action? primaryAction,
        Action? cancelAction = null,
        Action? secondaryAction = null,
        string title = "")
    {
        Application.Current.Dispatcher.BeginInvoke(
            method: new Action(() =>
            {
                MessageBoxButton messageBoxButton = MessageBoxButton.OKCancel;

                if (primaryAction == null)
                {
                    messageBoxButton = MessageBoxButton.OK;
                }

                if (secondaryAction != null)
                {
                    messageBoxButton = MessageBoxButton.YesNoCancel;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show(
                    owner: this,
                    messageBoxText: message,
                    caption: title,
                    button: messageBoxButton);

                switch (messageBoxResult)
                {
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        primaryAction?.Invoke();

                        break;
                    case MessageBoxResult.No:
                        if (messageBoxButton != MessageBoxButton.YesNoCancel)
                        {
                            cancelAction?.Invoke();
                        }
                        else
                        {
                            secondaryAction?.Invoke();
                        }

                        break;
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.None:
                        cancelAction?.Invoke();

                        break;
                }
            }),
            priority: DispatcherPriority.Normal);
    }

    /// <summary>
    /// 寫紀錄
    /// </summary>
    /// <param name="message">字串，訊息</param>
    /// <param name="title">字串，標題，預設值為空白</param>
    /// <param name="logEventLevel">LogEventLevel，預設值為 LogEventLevel.Information</param>
    [SuppressMessage("Performance", "CA1822:將成員標記為靜態", Justification = "<暫止>")]
    public void WriteLog(
        string message,
        LogEventLevel logEventLevel = LogEventLevel.Information) =>
            CustomFunction.WriteLog(
                message,
                logEventLevel);

    /// <summary>
    /// 取得 HttpClient
    /// </summary>
    /// <returns>HttpClient</returns>
    public static HttpClient? GetHttpClient()
    {
        HttpClient? httpClient = GlobalHCFactory?.CreateClient();

        // 參考來源：https://github.com/RayWangQvQ/BiliBiliToolPro/pull/350
        // 設定 HttpClient 的標頭資訊。
        //httpClient?.DefaultRequestHeaders.Referrer = new Uri("https://www.bilibili.com");
        //httpClient?.DefaultRequestHeaders.Add("Origin", "https://space.bilibili.com");

        httpClient?.DefaultRequestHeaders.Add("User-Agent", CustomFunction.GetUserAgent());
        httpClient?.DefaultRequestHeaders.Add("DNT", "1");

        ClientHintsUtil.SetClientHints(httpClient);

        return httpClient;
    }

    /// <summary>
    /// 自定義初始化
    /// </summary>
    private void CustomInit()
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsInitializing = true;

                // 設定應用的名稱。
                Title = MsgSet.AppName;

                GlobalDataSet.CollectionChanged += GlobalDataSet_CollectionChanged;

                // 日誌記錄需要使用等寬字型。
                RTBLog.FontFamily = AppLangUtil.GetLogFontFamily();

                // 初始化數個方法。
                // CustomFunction.Init() 必須第一個呼叫。
                CustomFunction.Init(this);
                // DownloaderUtil.Init() 必須在 ExternalProgram.Init() 之前呼叫。
                DownloaderUtil.Init(this);
                ExternalProgram.Init(this);
                DataGridExtension.Init(this);
                ClipListUtil.Init(this);
                DiscordRichPresenceUtil.Init(this);
                LyricsUtil.Init(this);
                DiscordRichPresenceUtil.Init(this);
                PlaywrightUtil.Init(this);
                B23ClipUtil.Init(this);
                OperationSet.Init(this);

                // 設定 FFmpeg 的路徑。
                FFmpeg.SetExecutablesPath(VariableSet.BinsFolderPath);

                // 初始化 OpenCC.NET。
                ZhConverter.Initialize();

                // 設定控制項。
                DGClipList.ItemsSource = GlobalDataSet;
                CBYTQuality.SelectedIndex = Properties.Settings.Default.MpvNetLibYTQualityIndex;
                CBPlaybackSpeed.SelectedIndex = Properties.Settings.Default.MpvNetLibPlaybackSpeedIndex;
                SVolume.Value = Convert.ToDouble(Properties.Settings.Default.MpvNetLibVolume.ToString());
                CBNoVideo.IsChecked = Properties.Settings.Default.MpvNetLibNoVideo;
                CBChromaKey.IsChecked = Properties.Settings.Default.MpvNetLibChromaKey;
                RBClipPlayer.IsChecked = true;
                CBAutoLyric.IsChecked = Properties.Settings.Default.NetPlaylistAutoLyric;
                MIFullDownloadFirst.IsChecked = Properties.Settings.Default.FullDownloadFirst;
                MIDeleteSourceFile.IsChecked = Properties.Settings.Default.DeleteSourceFile;
                TBCustomSubscriberAmount.Text = "-1";

                // 為 SSeek 加入 KeyUpEvent、KeyDownEvent 等事件處裡，以讓使用者可以透過方向鍵控制控制項。
                SSeek.AddHandler(KeyUpEvent, new KeyEventHandler(SSeek_KeyUpEvent), true);
                SSeek.AddHandler(KeyDownEvent, new KeyEventHandler(SSeek_KeyDownEvent), true);

                InitB23ClipListExcludedPhrases();

                TaskbarIconUtil.Init(this, TITaskbarIcon);

                Version? version = Assembly.GetExecutingAssembly().GetName().Version;

                if (version != null)
                {
                    LVersion.Content = version.ToString();
                }

                CBWhisperModel.Text = Properties.Settings.Default.WhisperModel;
                CBWhisperQuantization.Text = Properties.Settings.Default.WhisperQuantization;
                CBWhisperSamplingStrategy.Text = Properties.Settings.Default.WhisperSamplingStrategy;
                CBWhisperLanguage.Text = Properties.Settings.Default.WhisperLanguage;
                TBWhisperBeamSize.Text = Properties.Settings.Default.WhisperBeamSize.ToString();
                TBWhisperPatience.Text = Properties.Settings.Default.WhisperPatience.ToString();
                TBWhisperBestOf.Text = Properties.Settings.Default.WhisperBestOf.ToString();
                CBWhisperSpeedUp2x.IsChecked = Properties.Settings.Default.WhisperSpeedUp2x;
                CBWhisperTranslateToEnglish.IsChecked = Properties.Settings.Default.WhisperTranslateToEnglish;
                CBWhisperExportWebVTTAlso.IsChecked = Properties.Settings.Default.WhisperExportWebVTTAlso;

                ResourceDictionary? curResDict = AppLangUtil.GetCurrentLangResDict();

                string langCode = AppLangUtil.GetLangCode(curResDict);

                string[] targetLangArray =
                [
                    "en-US",
                    "en-GB"
                ];

                if (targetLangArray.Contains(langCode))
                {
                    // 針對特定與語系不啟用此按鈕。
                    CBUseTranslate.IsEnabled = false;
                }

                CBApplyFontSetting.IsChecked = Properties.Settings.Default.FFmpegApplyFontSetting;

                InitCBLanguages();
                InitUnsupportedDomains();

                TBUserAgent.Text = Properties.Settings.Default.UserAgent;
                TBSecChUa.Text = Properties.Settings.Default.SecChUa;
                TBAppendSeconds.Text = Properties.Settings.Default.PlaylistAppendSeconds.ToString();
                CBEnableMpvLogVerbose.IsChecked = Properties.Settings.Default.MpvNetLibLogVerbose;
                CBEnableDiscordRichPresence.IsChecked = Properties.Settings.Default.DiscordRichPresence;
                CBEnableOpenCCS2TWP.IsChecked = Properties.Settings.Default.OpenCCS2TWP;

                InitGpuDevice();

                CBEnableHardwareAcceleration.IsChecked = Properties.Settings.Default.FFmpegEnableHardwareAcceleration;

                InitFontList();

                ExternalProgram.CheckDependencyFiles(
                    new Action(() =>
                    {
                        InitEncodingList();

                        // 載入 yt-dlp.conf 的設定值。
                        LoadYtDlpConf();

                        // 初始化 MpvPlayer。
                        InitMpvPlayer(PlayerHost.Handle);

                        TTTips.SetToolTip(
                            PlayerHost,
                            MsgSet.MsgDoubleClickToTogglePopupWindow);

                        // 根據設定值決定是否要初始化 Discord 豐富狀態。
                        if (Properties.Settings.Default.DiscordRichPresence)
                        {
                            DiscordRichPresenceUtil.InitRichPresence();
                        }

                        // 延後 1.5 秒後再執行。
                        Task.Delay(1500).ContinueWith(t =>
                        {
                            IsInitializing = false;

                            // 自動載入短片清單檔案。
                            DoAutoLoadClipLists();

                            // 載入網路資源。
                            InitNetResurce();
                        });
                    }));

                // 檢查應用程式是否有新版本。
                CheckAppVersion();
            }));
        }
        catch (Exception ex)
        {
            WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 執行自動載入短片清單檔案
    /// </summary>
    private void DoAutoLoadClipLists()
    {
        try
        {
            List<string> files = CustomFunction
                .EnumerateFiles(
                    VariableSet.ClipListsFolderPath,
                    VariableSet.AllowedExts,
                    SearchOption.TopDirectoryOnly)
                .ToList();

            DoLoadClipLists(files);
        }
        catch (Exception ex)
        {
            WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 執行載入短片清單檔案
    /// </summary>
    /// <param name="files">List&lt;string&gt;</param>
    private void DoLoadClipLists(List<string> files)
    {
        try
        {
            foreach (string path in files)
            {
                string extName = Path.GetExtension(path),
                    rawJson = File.ReadAllText(path);

                switch (extName)
                {
                    case ".json":
                        List<ClipData>? dataSource1 = JsonSerializer
                            .Deserialize<List<ClipData>>(rawJson, VariableSet.SharedJSOptions);

                        List<TimestampSongData>? dataSource2 = JsonSerializer
                            .Deserialize<List<TimestampSongData>>(rawJson, VariableSet.SharedJSOptions);

                        List<SecondsSongData>? dataSource3 = JsonSerializer
                            .Deserialize<List<SecondsSongData>>(rawJson, VariableSet.SharedJSOptions);

                        ClipData? data1 = dataSource1?.FirstOrDefault();
                        TimestampSongData? data2 = dataSource2?.FirstOrDefault();
                        SecondsSongData? data3 = dataSource3?.FirstOrDefault();

                        // 判斷讀取到的檔案是否為短片清單檔案。
                        if (!string.IsNullOrEmpty(data1?.VideoUrlOrID) &&
                            (data2?.VideoID == null || data3?.VideoID == null))
                        {
                            DGClipList.ImportData(dataSource1);
                        }
                        else
                        {
                            TimeSpan? timeSpan = data2?.StartTime;

                            double? seconds = data3?.StartSeconds;

                            // 當 timeSpan 不為 null 時，則表示讀取到的是時間標記播放清單檔案。
                            if (timeSpan != null)
                            {
                                DGClipList.ImportData(dataSource2);
                            }
                            else
                            {
                                // 當 seconds 為 null 或大於等於 0 時，則表示讀取到的是秒數播放清單檔案。
                                if (seconds == null || seconds >= 0)
                                {
                                    DGClipList.ImportData(dataSource3);
                                }
                            }
                        }

                        break;
                    case ".jsonc":
                        // 支援來自：https://github.com/YoutubeClipPlaylist/Playlists 的 *.jsonc 檔案。
                        List<List<object>>? dataSource4 = JsonSerializer
                            .Deserialize<List<List<object>>>(rawJson, VariableSet.SharedJSOptions);

                        DGClipList.ImportData(dataSource4);

                        break;
                    case ".txt":
                        DGClipList.ImportData(path);

                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 取得 DGClipList 所選擇的項目
    /// </summary>
    /// <returns>ClipData?</returns>
    private ClipData? GetDGClipListSelectedItem()
    {
        if (DGClipList.SelectedItem != null)
        {
            if (DGClipList.CurrentCell.Item is ClipData clipData)
            {
                return clipData;
            }
        }

        return null;
    }

    /// <summary>
    /// 取得與指定 ClipData 有關的 ClipData 資料
    /// </summary>
    /// <returns>List&lt;ClipData&gt;?</returns>
    private List<ClipData>? GetClipDataRelatedItems(ClipData? clipData)
    {
        if (clipData != null)
        {
            return
            [
                .. GlobalDataSet.Where(n => n.VideoUrlOrID == clipData.VideoUrlOrID)
                    .OrderBy(n => n.No)
            ];
        }

        return null;
    }

    /// <summary>
    /// 取得全部的 ClipData（有排序）
    /// </summary>
    /// <returns>List&lt;ClipData&gt;</returns>
    private List<ClipData> GetAllClipDatas()
    {
        return
        [
            .. GlobalDataSet.OrderBy(n => n.No)
        ];
    }

    /// <summary>
    /// 取得已分組的全部 ClipData（有排序）
    /// </summary>
    /// <returns>List&lt;IOrderedEnumerable&lt;ClipData&gt;&gt;</returns>
    private List<IOrderedEnumerable<ClipData>> GetGroupedAllClipDatas()
    {
        return GlobalDataSet
            .GroupBy(n => n.VideoUrlOrID)
            .Select(n => n.OrderBy(m => m.No))
            .ToList();
    }

    /// <summary>
    /// 取得 GlobalCT
    /// </summary>
    /// <returns>CancellationToken</returns>
    private CancellationToken GetGlobalCT()
    {
        GlobalCTS ??= new();
        GlobalCT = GlobalCTS.Token;

        return GlobalCT.Value;
    }

    /// <summary>
    /// 執行隨機排列短片清單
    /// </summary>
    private void DoReorderClipList()
    {
        try
        {
            for (int i = 0; i < GlobalDataSet.Count; i++)
            {
                ClipData currentClipData = GlobalDataSet[i];

                int randomIndex = RandomNumberGenerator.GetInt32(0, GlobalDataSet.Count - 1);

                GlobalDataSet[i] = GlobalDataSet[randomIndex];
                GlobalDataSet[randomIndex] = currentClipData;
            }

            // 排序 DGClipList。
            Dispatcher.BeginInvoke(() =>
            {
                SortState sortState = GetSortState();

                DGClipList.Sort(sortState.ColumnIndex, sortState.SortDirection);
            });
        }
        catch (Exception ex)
        {
            WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 執行更新 GlobalDataSet 各筆資料的 No
    /// </summary>
    private void DoUpdateNoInGlobalDataSet()
    {
        try
        {
            // 除非未播放短片，否則 curClipData 不會為 null。
            ClipData? curClipData = CPPlayer.ClipData;

            if (curClipData != null)
            {
                int newIndex = GlobalDataSet.IndexOf(curClipData);

                CPPlayer.UpdateIndex(
                    DGClipList,
                    GlobalDataSet,
                    newIndex);
            }

            // 更新短片清單現有資料的 No。
            int newNo = 1;

            foreach (ClipData clipData in GlobalDataSet)
            {
                clipData.No = newNo;

                newNo++;
            }
        }
        catch (Exception ex)
        {
            WriteLog(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgErrorOccured,
                    ex.GetExceptionMessage()),
                logEventLevel: LogEventLevel.Error);
        }
    }

    /// <summary>
    /// 關閉應用程式
    /// </summary>
    /// <param name="sender">object，sender</param>
    private void ShutdownApp(object? sender)
    {
        Action baseAction = new(() =>
        {
            // 執行短片工具的取消按鈕。
            MICancel_Click(sender, new RoutedEventArgs(ButtonBase.ClickEvent));

            // 執行短片播放器的停止按鈕。
            BtnStop_Click(sender, new RoutedEventArgs(ButtonBase.ClickEvent));

            // 執行清除方法。
            MPPlayer?.Dispose();
            TaskbarIconUtil.Dispose();
            DiscordRichPresenceUtil.Dispose();

            // 清除變數值。
            GlobalDRClient = null;

            // 用於在應用程式結束前儲存特定 Grid 的
            // RowDefinition、ColumnDefinition 的值。
            Properties.Settings.Default.Save();
        });

        ShowConfirmMsgBox(
            message: MsgSet.MsgConfirmExitApp,
            primaryAction: new Action(async () =>
            {
                try
                {
                    await Dispatcher.BeginInvoke(async () =>
                    {
                        IsByPassingWindowClosingEventCancel = true;

                        baseAction.Invoke();

                        // 先刪除舊檔案。
                        if (File.Exists(VariableSet.TempClipListFilePath))
                        {
                            File.Delete(VariableSet.TempClipListFilePath);
                        }

                        ObservableCollection<ClipData>? dataSource = DGClipList.GetDataSource();

                        // 當有資料時再重建檔案。
                        if (dataSource?.Count > 0)
                        {
                            using FileStream fileStream = new(
                                VariableSet.TempClipListFilePath,
                                new FileStreamOptions()
                                {
                                    Access = FileAccess.ReadWrite,
                                    Mode = FileMode.OpenOrCreate,
                                    Share = FileShare.ReadWrite
                                });

                            await JsonSerializer.SerializeAsync(
                                fileStream,
                                dataSource,
                                VariableSet.SharedJSOptions);

                            await fileStream.DisposeAsync();
                        }

                        // 手動結束應用程式。
                        Application.Current.Shutdown();
                    });
                }
                catch (Exception ex)
                {
                    WriteLog(
                        message: MsgSet.GetFmtStr(
                            MsgSet.MsgErrorOccured,
                            ex.GetExceptionMessage()),
                        logEventLevel: LogEventLevel.Error);
                }
            }),
            secondaryAction: new Action(() =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    IsByPassingWindowClosingEventCancel = true;

                    baseAction.Invoke();

                    // 手動結束應用程式。
                    Application.Current.Shutdown();
                });
            }));
    }

    /// <summary>
    /// 檢查應用程式的版本
    /// </summary>
    private async void CheckAppVersion()
    {
        using HttpClient? HttpClient = GetHttpClient();

        if (HttpClient == null)
        {
            return;
        }

        CheckResult checkResult = await UpdateNotifier.CheckVersion(HttpClient);

        if (!string.IsNullOrEmpty(checkResult.MessageText))
        {
            WriteLog(checkResult.MessageText);
        }

        if (checkResult.HasNewVersion &&
            !string.IsNullOrEmpty(checkResult.DownloadUrl))
        {
            ShowConfirmMsgBox(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgUpdateNotifierDownloadNewVersion,
                    checkResult.VersionText ?? string.Empty),
                primaryAction: new Action(() =>
                {
                    CustomFunction.OpenUrl(checkResult.DownloadUrl);
                }));
        }

        if (checkResult.NetVersionIsOdler &&
            !string.IsNullOrEmpty(checkResult.DownloadUrl))
        {
            ShowConfirmMsgBox(
                message: MsgSet.GetFmtStr(
                    MsgSet.MsgUpdateNotifierDownloadOldVersion,
                    checkResult.VersionText ?? string.Empty),
                primaryAction: new Action(() =>
                {
                    CustomFunction.OpenUrl(checkResult.DownloadUrl);
                }));
        }
    }

    /// <summary>
    /// 取得在已排序的 GlobalDataSet 中的實際索引值
    /// </summary>
    /// <param name="clipData">ClipData</param>
    /// <returns>數值，索引值</returns>
    private int GetActualIndex(ClipData? clipData)
    {
        return clipData == null ? -1 : GetSortedGlobalDataSet().IndexOf(clipData);
    }

    /// <summary>
    /// 取得 DGClipList 的排序狀態
    /// </summary>
    /// <returns>SortState</returns>
    private SortState GetSortState()
    {
        ListSortDirection? targetSortDirection = null;

        int columnIndex = -1;

        // 理論上同時間只會有一個 ListSortDirection。
        for (int i = 0; i < DGClipList.Columns.Count; i++)
        {
            ListSortDirection? sortDirection = DGClipList.Columns[i].SortDirection;

            if (sortDirection != null)
            {
                columnIndex = i;

                targetSortDirection = sortDirection;

                break;
            }
        }

        return new SortState()
        {
            ColumnIndex = columnIndex,
            SortDirection = targetSortDirection
        };
    }

    /// <summary>
    /// 取得已排序的 GlobalDataSet
    /// </summary>
    /// <returns>List&lt;ClipData&gt;</returns>
    private List<ClipData> GetSortedGlobalDataSet()
    {
        List<ClipData> sortedGlobalDataSet = [];

        // 來源：https://stackoverflow.com/a/7333624
        ICollectionView collectionView = CollectionViewSource
            .GetDefaultView(DGClipList.ItemsSource);

        foreach (object? item in collectionView)
        {
            if (item is ClipData clipData)
            {
                if (clipData != null)
                {
                    sortedGlobalDataSet.Add(clipData);
                }
            }
        }

        return sortedGlobalDataSet;
    }
}