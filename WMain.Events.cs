using Brushes = System.Windows.Media.Brushes;
using Control = System.Windows.Controls.Control;
using CustomToolbox.Common;
using static CustomToolbox.Common.Sets.EnumSet;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Models.ImportPlaylist;
using CustomToolbox.Common.Sets;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using H.NotifyIcon;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using RichTextBox = System.Windows.Controls.RichTextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TabControl = System.Windows.Controls.TabControl;

namespace CustomToolbox;

/// <summary>
/// WMain 的控制項事件
/// </summary>
public partial class WMain : Window
{
    private void GlobalDataSet_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        // 產生新的 No。
                        int newInsertNo = e.NewStartingIndex;

                        newInsertNo++;

                        // 理論上 e.NewItems 只會有一筆資料。
                        foreach (ClipData newClipData in e.NewItems)
                        {
                            // 當新加入資料的 No 為 0 時，才更新 No。
                            if (newClipData.No == 0)
                            {
                                newClipData.No = newInsertNo;

                                newInsertNo++;
                            }
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    DoUpdateNoInGlobalDataSet();

                    break;
                case NotifyCollectionChangedAction.Replace:
                    DoUpdateNoInGlobalDataSet();

                    break;
                default:
                    break;
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

    private void MILoadClipListFile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = MsgSet.LoadClipListSelectFile,
                Filter = MsgSet.LoadClipListFilter,
                FilterIndex = 1,
                InitialDirectory = VariableSet.ClipListsFolderPath
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName,
                    rawJson = File.ReadAllText(filePath);

                switch (openFileDialog.FilterIndex)
                {
                    case 1:
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
                    case 2:
                        // 支援來自：https://github.com/YoutubeClipPlaylist/Playlists 的 *.jsonc 檔案。
                        List<List<object>>? dataSource4 = JsonSerializer
                            .Deserialize<List<List<object>>>(rawJson, VariableSet.SharedJSOptions);

                        DGClipList.ImportData(dataSource4);

                        break;
                    case 3:
                        DGClipList.ImportData(filePath);

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

    private async void MISaveClipListFile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ObservableCollection<ClipData>? dataSource = DGClipList.GetDataSource();

            if (dataSource == null || dataSource.Count <= 0)
            {
                WriteLog(message: MsgSet.MsgCanNotSaveClipList);

                return;
            }

            string dateString = $"{DateTime.Now:yyyyMMdd}";
            string defaultFileName = $"Playlist_{dateString}";

            SaveFileDialog saveFileDialog = new()
            {
                Title = MsgSet.SaveClipListSelectFile,
                FileName = defaultFileName,
                Filter = MsgSet.SaveClipListFilter,
                FilterIndex = 1,
                AddExtension = true,
                InitialDirectory = VariableSet.ClipListsFolderPath
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string originFilePath = saveFileDialog.FileName;
                string originFileName = Path.GetFileNameWithoutExtension(originFilePath);

                // 當檔案名稱為預設名稱的時候，才會變更儲存的檔案名稱。
                if (originFilePath.Contains(defaultFileName))
                {
                    // 變更檔案名稱。
                    saveFileDialog.FileName = saveFileDialog.FilterIndex switch
                    {
                        1 => originFilePath.Replace(originFileName, $"ClipList_{dateString}"),
                        2 => originFilePath.Replace(originFileName, $"Playlist_Timestamps_{dateString}"),
                        3 => originFilePath.Replace(originFileName, $"Playlist_Seconds_{dateString}"),
                        4 => originFilePath.Replace(originFileName, $"SongList_{dateString}"),
                        5 => originFilePath.Replace(originFileName, $"Exported_timestamps_{dateString}"),
                        _ => saveFileDialog.FileName
                    };
                }

                using FileStream fileStream = (FileStream)saveFileDialog.OpenFile();

                string outputContent = string.Empty;

                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        await JsonSerializer.SerializeAsync(
                            fileStream,
                            dataSource,
                            VariableSet.SharedJSOptions);

                        break;
                    case 2:
                        List<TimestampSongData> outputDataSource1 = [];

                        foreach (ClipData clipData in dataSource)
                        {
                            outputDataSource1.Add(new TimestampSongData()
                            {
                                VideoID = clipData.VideoUrlOrID,
                                Name = clipData.Name,
                                StartTime = clipData.StartTime,
                                EndTime = clipData.EndTime,
                                SubSrc = clipData.SubtitleFileUrl
                            });
                        }

                        await JsonSerializer.SerializeAsync(
                            fileStream,
                            outputDataSource1,
                            VariableSet.SharedJSOptions);

                        break;
                    case 3:
                        List<SecondsSongData> outputDataSource2 = [];

                        foreach (ClipData clipData in dataSource)
                        {
                            outputDataSource2.Add(new SecondsSongData()
                            {
                                VideoID = clipData.VideoUrlOrID,
                                Name = clipData.Name,
                                StartSeconds = clipData.StartTime.TotalSeconds,
                                EndSeconds = clipData.EndTime.TotalSeconds,
                                SubSrc = clipData.SubtitleFileUrl
                            });
                        }

                        await JsonSerializer.SerializeAsync(
                            fileStream,
                            outputDataSource2,
                            VariableSet.SharedJSOptions);

                        break;
                    case 4:
                        List<List<object>> outputDataSource3 = [];

                        foreach (ClipData clipData in dataSource)
                        {
                            outputDataSource3.Add(
                            [
                                clipData.VideoUrlOrID ?? string.Empty,
                                clipData.StartTime.TotalSeconds,
                                clipData.EndTime.TotalSeconds,
                                clipData.Name ?? string.Empty,
                                clipData.SubtitleFileUrl ?? string.Empty
                            ]);
                        }

                        await JsonSerializer.SerializeAsync(
                            fileStream,
                            outputDataSource3,
                            VariableSet.SharedJSOptions);

                        break;
                    case 5:
                        // ※輸出的備註內容需保持正體中文。 

                        var grpClipDataSet = dataSource.GroupBy(n => n.VideoUrlOrID);

                        foreach (var grpClipData in grpClipDataSet)
                        {
                            int countIndex2 = 0;

                            foreach (ClipData clipData in grpClipData)
                            {
                                if (countIndex2 == 0)
                                {
                                    if (grpClipData.Key?.StartsWith("http") == true)
                                    {
                                        outputContent += VariableSet.TimestampHeaderTemplate
                                            .Replace("https://www.youtube.com/watch?v={VideoID}", grpClipData.Key);
                                    }
                                    else
                                    {
                                        outputContent += VariableSet.TimestampHeaderTemplate
                                            .Replace("{VideoID}", grpClipData.Key);
                                    }

                                    outputContent += $"{VariableSet.Timestamp.StartReadingToken}{Environment.NewLine}";
                                }

                                // 不輸出字幕檔來源。
                                outputContent += $"{VariableSet.Timestamp.CommentPrefixToken} " +
                                    $"{clipData.Name}{VariableSet.Timestamp.CommentSuffixToken1}" +
                                    $"{Environment.NewLine}{(clipData.StartTime.ToTimestampString())}" +
                                    $"{Environment.NewLine}{VariableSet.Timestamp.CommentPrefixToken} " +
                                    $"{clipData.Name}{VariableSet.Timestamp.CommentSuffixToken2}" +
                                    $"{Environment.NewLine}{(clipData.EndTime.ToTimestampString())}" +
                                    $"{Environment.NewLine}{Environment.NewLine}";

                                countIndex2++;
                            }

                            if (countIndex2 > 0 && countIndex2 < grpClipDataSet.Count() - 1)
                            {
                                // 分隔用行。
                                outputContent += $"{VariableSet.Timestamp.BlockSeparator}" +
                                    $"{Environment.NewLine}{Environment.NewLine}";
                            }
                        }

                        break;
                    default:
                        break;
                }

                switch (saveFileDialog.FilterIndex)
                {
                    case 4:
                        {
                            StringBuilder stringBuilder = new();

                            using StreamReader streamReader = new(fileStream);

                            stringBuilder.Append(VariableSet.JsoncHeader);

                            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                            while (!streamReader.EndOfStream)
                            {
                                stringBuilder.AppendLine(streamReader.ReadLine());
                            }

                            await fileStream.DisposeAsync();

                            using StreamWriter streamWriter = new(saveFileDialog.FileName);

                            streamWriter.Write(stringBuilder);

                            await streamWriter.DisposeAsync();
                        }

                        break;
                    case 5:
                        {
                            using StreamWriter streamWriter = new(fileStream);

                            streamWriter.Write(outputContent);

                            await streamWriter.DisposeAsync();
                            await fileStream.DisposeAsync();
                        }

                        break;
                    default:
                        await fileStream.DisposeAsync();

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

    #region 資料夾

    public void MIOpenBinsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.BinsFolderPath);
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

    public void MIOpenConfigFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 來源：https://stackoverflow.com/a/7069366
            string configFilePath = ConfigurationManager
                .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,
                fileName = Path.GetFileName(configFilePath),
                folderPath = Path.GetFullPath(configFilePath).Replace(fileName, string.Empty);

            CustomFunction.OpenFolder(folderPath);
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

    public void MIOpenDownloadsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.DownloadsFolderPath);
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

    public void MIOpenCliplistsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.ClipListsFolderPath);
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

    public void MIOpenLogsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.LogsFolderPath);
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

    public void MIOpenLyricsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.LyricsFolderPath);
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

    public void MIOpenTempFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.TempFolderPath);
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

    public void MIOpenModelsFolder_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenFolder(VariableSet.ModelsFolderPath);
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

    #endregion

    public void MIExit_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 強制顯示應用程式以顯示對話視窗。
                WindowExtensions.Show(
                    this,
                    disableEfficiencyMode: true);

                WindowState = WindowState.Normal;

                ShutdownApp(sender);
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

    private void MICancel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                bool value = MICancel.IsEnabled;

                if (value)
                {
                    MICancel.Foreground = Brushes.Red;
                }
                else
                {
                    MICancel.Foreground = Brushes.Gray;
                }
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

    private void MICancel_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (GlobalCTS != null &&
                    !GlobalCTS.IsCancellationRequested)
                {
                    GlobalCTS.Cancel();

                    // 清除變數。
                    GlobalCTS = null;
                    GlobalCT = null;

                    // 重設控制項。
                    OperationSet.ResetControls();
                }
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

    public void MICheckUpdate_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 強制顯示應用程式以顯示對話視窗。
                WindowExtensions.Show(
                    this,
                    disableEfficiencyMode: true);

                // 只有在 WindowState 是 WindowState.Minimized 時，
                // 才重新設定 WindowState 至 WindowState.Normal。
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }

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

    public void MIAbout_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Version? version = Assembly.GetExecutingAssembly().GetName().Version;

                if (version != null)
                {
                    // 強制顯示應用程式以顯示對話視窗。
                    WindowExtensions.Show(
                        this,
                        disableEfficiencyMode: true);

                    // 只有在 WindowState 是 WindowState.Minimized 時，
                    // 才重新設定 WindowState 至 WindowState.Normal。
                    if (WindowState == WindowState.Minimized)
                    {
                        WindowState = WindowState.Normal;
                    }

                    string appAbout = MsgSet.GetFmtStr(
                        MsgSet.AppAbout,
                        MsgSet.AppName,
                        version.ToString(),
                        MsgSet.AppDescription);

                    ShowMsgBox(appAbout, MsgSet.About);
                }
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

    private void TCTabBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            // 2022/12/12 暫時先不利用。

            TabControl? control = (TabControl)sender;
            TabItem? tabItem = (TabItem?)control?.SelectedItem;

            string value = tabItem?.Name ?? string.Empty;

            switch (value)
            {
                case nameof(TIClipPlayer):
                    break;
                case nameof(TINetResource):
                    break;
                case nameof(TIClipTool):
                    break;
                case nameof(TIOtherTool):
                    break;
                case nameof(TIMisc):
                    break;
                default:
                    break;
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

    private void DGClipList_DragEnter(object sender, DragEventArgs e)
    {
        try
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
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

    private void DGClipList_Drop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) != null)
                {
                    List<string>? files = ((string[]?)e.Data.GetData(DataFormats.FileDrop))
                        ?.Where(n => VariableSet.AllowedExts.Contains(Path.GetExtension(n)))
                        .ToList();

                    if (files != null)
                    {
                        if (files.Count == 0)
                        {
                            ShowMsgBox(MsgSet.MsgSelectAValidClipListFile);

                            return;
                        }

                        DoLoadClipLists(files);
                    }
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

    private void DGClipList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (sender is DataGrid dataGrid)
            {
                if (dataGrid.SelectedItem != null)
                {
                    if (dataGrid.CurrentCell.Item is ClipData clipData &&
                        !string.IsNullOrEmpty(clipData.VideoUrlOrID))
                    {
                        PlayClip(clipData);
                    }
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

    private void MIPlayClip_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ClipData? clipData = GetDGClipListSelectedItem();

            if (clipData != null)
            {
                if (string.IsNullOrEmpty(clipData.VideoUrlOrID) ||
                    !CustomFunction.IsSupportDomain(clipData.VideoUrlOrID))
                {
                    ShowMsgBox(MsgSet.MsgUnSupportedSite);

                    return;
                }

                PlayClip(clipData);
            }
            else
            {
                ShowMsgBox(MsgSet.MsgSelectTheClipToPlay);
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

    private async void MIFetchClip_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet1 =
            [
                MIFetchClip,
                MIDLClip,
                MIDLClipsByTheSameUrl,
                MIBatchDLClips,
                BtnGenerateB23ClipList,
                BtnBurnInSubtitle,
                BtnSplitVideo,
                BtnWhisperDetectVideoLanguage,
                BtnWhisperTranscribeVideo,
                BtnYtscToolTakeScreenshot
            ];

            Control[] ctrlSet2 =
            [
                MICancel
            ];

            CustomFunction.BatchSetEnabled(ctrlSet1, false);
            CustomFunction.BatchSetEnabled(ctrlSet2, true);

            // 先清除日誌紀錄。
            MIClearLog_Click(sender, e);

            ClipData? clipData = GetDGClipListSelectedItem();

            if (clipData != null && !string.IsNullOrEmpty(clipData.VideoUrlOrID))
            {
                await OperationSet.DoFetchClipInfo(clipData.VideoUrlOrID, GetGlobalCT());
            }
            else
            {
                ShowMsgBox(MsgSet.MsgSelectTheClipToFetchInfo);
            }

            CustomFunction.BatchSetEnabled(ctrlSet1, true);
            CustomFunction.BatchSetEnabled(ctrlSet2, false);
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

    private void MIDLClip_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                Control[] ctrlSet1 =
                [
                    MIFetchClip,
                    MIDLClip,
                    MIDLClipsByTheSameUrl,
                    MIBatchDLClips,
                    MIFullDownloadFirst,
                    MIDeleteSourceFile,
                    BtnGenerateB23ClipList,
                    BtnBurnInSubtitle,
                    BtnSplitVideo,
                    BtnWhisperDetectVideoLanguage,
                    BtnWhisperTranscribeVideo,
                    BtnYtscToolTakeScreenshot
                ];

                Control[] ctrlSet2 =
                [
                    MICancel
                ];

                CustomFunction.BatchSetEnabled(ctrlSet1, false);
                CustomFunction.BatchSetEnabled(ctrlSet2, true);

                ClipData? clipData = GetDGClipListSelectedItem();

                if (clipData != null)
                {
                    bool useHardwareAcceleration = Properties.Settings.Default
                            .FFmpegEnableHardwareAcceleration,
                        isFullDownloadFirst = Properties.Settings.Default.FullDownloadFirst,
                        isDeleteSourceFile = Properties.Settings.Default.DeleteSourceFile;

                    HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel;

                    if (!string.IsNullOrEmpty(CBHardwareAccelerationType.Text))
                    {
                        hardwareAcceleratorType = CBHardwareAccelerationType.Text switch
                        {
                            nameof(HardwareAcceleratorType.AMD) => HardwareAcceleratorType.AMD,
                            nameof(HardwareAcceleratorType.Intel) => HardwareAcceleratorType.Intel,
                            nameof(HardwareAcceleratorType.NVIDIA) => HardwareAcceleratorType.NVIDIA,
                            _ => HardwareAcceleratorType.Intel
                        };
                    }

                    VideoCardData videoCardData = (VideoCardData)CBGpuDevice.SelectedItem;

                    int deviceNo = videoCardData.DeviceNo;

                    await OperationSet.DoDownloadClip(
                        clipData: clipData,
                        isFullDownloadFirst: isFullDownloadFirst,
                        useHardwareAcceleration: useHardwareAcceleration,
                        hardwareAcceleratorType: hardwareAcceleratorType,
                        deviceNo: deviceNo,
                        isDeleteSourceFile: isDeleteSourceFile,
                        ct: GetGlobalCT());
                }
                else
                {
                    ShowMsgBox(MsgSet.MsgSelectTheClipToDownload);
                }

                CustomFunction.BatchSetEnabled(ctrlSet1, true);
                CustomFunction.BatchSetEnabled(ctrlSet2, false);
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

    private void MIDLClipsByTheSameUrl_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                Control[] ctrlSet1 =
                [
                    MIFetchClip,
                    MIDLClip,
                    MIDLClipsByTheSameUrl,
                    MIBatchDLClips,
                    MIFullDownloadFirst,
                    MIDeleteSourceFile,
                    BtnGenerateB23ClipList,
                    BtnBurnInSubtitle,
                    BtnSplitVideo,
                    BtnWhisperDetectVideoLanguage,
                    BtnWhisperTranscribeVideo,
                    BtnYtscToolTakeScreenshot
                ];

                Control[] ctrlSet2 =
                [
                    MICancel
                ];

                CustomFunction.BatchSetEnabled(ctrlSet1, false);
                CustomFunction.BatchSetEnabled(ctrlSet2, true);

                ClipData? clipData = GetDGClipListSelectedItem();

                if (clipData != null)
                {
                    List<ClipData>? clipDatas = GetClipDataRelatedItems(clipData);

                    if (clipDatas == null || clipDatas.Count == 0)
                    {
                        ShowMsgBox(MsgSet.MsgCantFindTheSameUrlClips);

                        CustomFunction.BatchSetEnabled(ctrlSet1, true);
                        CustomFunction.BatchSetEnabled(ctrlSet2, false);

                        return;
                    }

                    bool useHardwareAcceleration = Properties.Settings.Default
                        .FFmpegEnableHardwareAcceleration,
                    isFullDownloadFirst = Properties.Settings.Default.FullDownloadFirst,
                    isDeleteSourceFile = Properties.Settings.Default.DeleteSourceFile;

                    HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel;

                    if (!string.IsNullOrEmpty(CBHardwareAccelerationType.Text))
                    {
                        hardwareAcceleratorType = CBHardwareAccelerationType.Text switch
                        {
                            nameof(HardwareAcceleratorType.AMD) => HardwareAcceleratorType.AMD,
                            nameof(HardwareAcceleratorType.Intel) => HardwareAcceleratorType.Intel,
                            nameof(HardwareAcceleratorType.NVIDIA) => HardwareAcceleratorType.NVIDIA,
                            _ => HardwareAcceleratorType.Intel
                        };
                    }

                    VideoCardData videoCardData = (VideoCardData)CBGpuDevice.SelectedItem;

                    int deviceNo = videoCardData.DeviceNo;

                    // 判斷是否為先下載完整短片。
                    if (isFullDownloadFirst)
                    {
                        await OperationSet.DoDownloadClips(
                            control: DGClipList,
                            clipData: clipData,
                            clipDatas: clipDatas,
                            isFullDownloadFirst: isFullDownloadFirst,
                            useHardwareAcceleration: useHardwareAcceleration,
                            hardwareAcceleratorType: hardwareAcceleratorType,
                            deviceNo: deviceNo,
                            isDeleteSourceFile: isDeleteSourceFile,
                            ct: GetGlobalCT());
                    }
                    else
                    {
                        await OperationSet.DoDownloadClips(
                            control: DGClipList,
                            clipDatas: clipDatas,
                            isFullDownloadFirst: isFullDownloadFirst,
                            useHardwareAcceleration: useHardwareAcceleration,
                            hardwareAcceleratorType: hardwareAcceleratorType,
                            deviceNo: deviceNo,
                            isDeleteSourceFile: isDeleteSourceFile,
                            ct: GetGlobalCT());
                    }
                }
                else
                {
                    ShowMsgBox(MsgSet.MsgSelectTheClipToDownload);
                }

                CustomFunction.BatchSetEnabled(ctrlSet1, true);
                CustomFunction.BatchSetEnabled(ctrlSet2, false);
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

    private void MIBatchDLClips_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                Control[] ctrlSet1 =
                [
                    MIFetchClip,
                    MIDLClip,
                    MIDLClipsByTheSameUrl,
                    MIBatchDLClips,
                    MIFullDownloadFirst,
                    MIDeleteSourceFile,
                    BtnGenerateB23ClipList,
                    BtnBurnInSubtitle,
                    BtnSplitVideo,
                    BtnWhisperDetectVideoLanguage,
                    BtnWhisperTranscribeVideo,
                    BtnYtscToolTakeScreenshot
                ];

                Control[] ctrlSet2 =
                [
                    MICancel
                ];

                CustomFunction.BatchSetEnabled(ctrlSet1, false);
                CustomFunction.BatchSetEnabled(ctrlSet2, true);

                List<IOrderedEnumerable<ClipData>> clipDatas = GetGroupedAllClipDatas();

                if (clipDatas.Count == 0)
                {
                    ShowMsgBox(MsgSet.MsgClipListNoData);

                    CustomFunction.BatchSetEnabled(ctrlSet1, true);
                    CustomFunction.BatchSetEnabled(ctrlSet2, false);

                    return;
                }

                bool useHardwareAcceleration = Properties.Settings.Default
                    .FFmpegEnableHardwareAcceleration,
                isFullDownloadFirst = Properties.Settings.Default.FullDownloadFirst,
                isDeleteSourceFile = Properties.Settings.Default.DeleteSourceFile;

                HardwareAcceleratorType hardwareAcceleratorType = HardwareAcceleratorType.Intel;

                if (!string.IsNullOrEmpty(CBHardwareAccelerationType.Text))
                {
                    hardwareAcceleratorType = CBHardwareAccelerationType.Text switch
                    {
                        nameof(HardwareAcceleratorType.AMD) => HardwareAcceleratorType.AMD,
                        nameof(HardwareAcceleratorType.Intel) => HardwareAcceleratorType.Intel,
                        nameof(HardwareAcceleratorType.NVIDIA) => HardwareAcceleratorType.NVIDIA,
                        _ => HardwareAcceleratorType.Intel
                    };
                }

                VideoCardData videoCardData = (VideoCardData)CBGpuDevice.SelectedItem;

                int deviceNo = videoCardData.DeviceNo;

                foreach (IOrderedEnumerable<ClipData> orderedClipDatas in clipDatas)
                {
                    List<ClipData> tempClipDatas = [.. orderedClipDatas];

                    ClipData? clipData = tempClipDatas.FirstOrDefault();

                    if (clipData != null)
                    {
                        // 判斷是否為先下載完整短片。
                        if (isFullDownloadFirst)
                        {
                            await OperationSet.DoDownloadClips(
                                control: DGClipList,
                                clipData: clipData,
                                clipDatas: tempClipDatas,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                isDeleteSourceFile: isDeleteSourceFile,
                                ct: GetGlobalCT());
                        }
                        else
                        {
                            await OperationSet.DoDownloadClips(
                                control: DGClipList,
                                clipDatas: tempClipDatas,
                                isFullDownloadFirst: isFullDownloadFirst,
                                useHardwareAcceleration: useHardwareAcceleration,
                                hardwareAcceleratorType: hardwareAcceleratorType,
                                deviceNo: deviceNo,
                                isDeleteSourceFile: isDeleteSourceFile,
                                ct: GetGlobalCT());
                        }
                    }
                }

                CustomFunction.BatchSetEnabled(ctrlSet1, true);
                CustomFunction.BatchSetEnabled(ctrlSet2, false);
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

    private void MIFullDownloadFirst_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    MenuItem control = (MenuItem)sender;

                    bool value = control.IsChecked;

                    if (Properties.Settings.Default.FullDownloadFirst != value)
                    {
                        Properties.Settings.Default.FullDownloadFirst = value;
                        Properties.Settings.Default.Save();
                    }
                }
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

    private void MIFullDownloadFirst_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    MenuItem control = (MenuItem)sender;

                    bool value = control.IsChecked;

                    if (Properties.Settings.Default.FullDownloadFirst != value)
                    {
                        Properties.Settings.Default.FullDownloadFirst = value;
                        Properties.Settings.Default.Save();
                    }
                }
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

    private void MIDeleteSourceFile_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    MenuItem control = (MenuItem)sender;

                    bool value = control.IsChecked;

                    if (Properties.Settings.Default.DeleteSourceFile != value)
                    {
                        Properties.Settings.Default.DeleteSourceFile = value;
                        Properties.Settings.Default.Save();
                    }
                }
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

    private void MIDeleteSourceFile_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    MenuItem control = (MenuItem)sender;

                    bool value = control.IsChecked;

                    if (Properties.Settings.Default.DeleteSourceFile != value)
                    {
                        Properties.Settings.Default.DeleteSourceFile = value;
                        Properties.Settings.Default.Save();
                    }
                }
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

    public void MIRandomPlayClip_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            DoRandomPlayClip();
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

    private void MIReorderClipList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            DoReorderClipList();
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

    private void MIClearClipList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            GlobalDataSet.Clear();
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

    private void MIClearLog_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                RTBLog.Document.Blocks.Clear();

                WriteLog(MsgSet.MsgLogCleared);
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

    private void MIExportLog_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    FileName = $"Log_{DateTime.Now:yyyyMMddHHmmss}",
                    Filter = MsgSet.ExportLogFilter,
                    FilterIndex = 1,
                    InitialDirectory = VariableSet.LogsFolderPath
                };

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    string textContent = new TextRange(
                        RTBLog.Document.ContentStart,
                        RTBLog.Document.ContentEnd).Text;

                    File.WriteAllText(
                        saveFileDialog.FileName,
                        textContent);

                    WriteLog(MsgSet.GetFmtStr(
                        MsgSet.MsgExportLogTo,
                        saveFileDialog.FileName));
                }
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

    private void RTBLog_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            RichTextBox? richTextBox = (RichTextBox?)sender;

            if (richTextBox == null)
            {
                return;
            }

            // Source: https://learn.microsoft.com/en-us/answers/questions/22612/wpf-auto-width-for-flowdocuments-content
            // Author: Alex Li-MSFT
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip,
                formattedTextWidth = richTextBox.Document
                    .GetFormattedText(pixelsPerDip)
                    .WidthIncludingTrailingWhitespace;

            richTextBox.Document.PageWidth = formattedTextWidth + 20;
            richTextBox.ScrollToEnd();
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
}