using Control = System.Windows.Controls.Control;
using CustomToolbox.Common;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using static CustomToolbox.Common.Sets.EnumSet;
using CustomToolbox.Common.Utils;
using H.NotifyIcon.Core;
using Mpv.NET.API;
using Mpv.NET.Player;
using Serilog.Events;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Globalization;

namespace CustomToolbox;

/// <summary>
/// MpvPlayer 的相關方法
/// </summary>
public partial class WMain
{
    /// <summary>
    /// 初始化 MpvPlayer
    /// </summary>
    /// <param name="hwnd">nint</param>
    private void InitMpvPlayer(nint hwnd)
    {
        try
        {
            MPPlayer = new MpvPlayer(hwnd, VariableSet.LibMpvPath)
            {
                Volume = Properties.Settings.Default.MpvNetLibVolume,
                Speed = CustomFunction.GetPlaybackSpeed(Properties.Settings.Default.MpvNetLibPlaybackSpeedIndex),
                YouTubeDlVideoQuality = CustomFunction.GetYTQuality(Properties.Settings.Default.MpvNetLibYTQualityIndex),
                LogLevel = GetMpvLogLeve()
            };

            if (MPPlayer != null)
            {
                MPPlayer.LoadConfig(VariableSet.MpvConfPath);
                MPPlayer.EnableYouTubeDl(VariableSet.YtDlHookLuaPath);

                if (Properties.Settings.Default.MpvNetLibNoVideo)
                {
                    MPPlayer.API.SetPropertyString("vid", "no");
                }
                else
                {
                    MPPlayer.API.SetPropertyString("vid", "auto");
                }

                if (Properties.Settings.Default.MpvNetLibChromaKey)
                {
                    MPPlayer.API.SetPropertyString("vf", VariableSet.VideoFilterColorize);
                }
                else
                {
                    MPPlayer.API.SetPropertyString("vf", VariableSet.VideoFilterNull);
                }

                MPPlayer.MediaLoaded += MediaLoaded;
                MPPlayer.MediaFinished += MediaFinished;
                MPPlayer.MediaPaused += MediaPaused;
                MPPlayer.MediaResumed += MediaResumed;
                MPPlayer.MediaStartedBuffering += MediaStartedBuffering;
                MPPlayer.MediaEndedBuffering += MediaEndedBuffering;
                MPPlayer.MediaStartedSeeking += MediaStartedSeeking;
                MPPlayer.MediaEndedSeeking += MediaEndedSeeking;
                MPPlayer.MediaError += MediaError;
                MPPlayer.MediaUnloaded += MediaUnloaded;
                MPPlayer.PositionChanged += PositionChanged;
                MPPlayer.API.LogMessage += LogMessage;
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
    /// 播放短片
    /// </summary>
    /// <param name="ClipData">ClipData，短片資料</param>
    private async void PlayClip(ClipData clipData)
    {
        try
        {
            if (MPPlayer != null)
            {
                // 讓 libmpv 先停止播放，以免在 yt-dlp 發生錯誤後，後續的新播放皆會失效。
                MPPlayer.Stop();

                string? path = clipData.VideoUrlOrID;

                if (path == null)
                {
                    string message = MsgSet.MsgInvalidVideoIDOrUrl;

                    WriteLog(message: message);

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Error);
                    TaskbarIconUtil.SetToolTip(string.Empty);

                    return;
                }

                UpdateClipPlayer(ClipPlayerStatus.Playing, clipData);

                if (!path.StartsWith("http"))
                {
                    path = $"https://www.youtube.com/watch?v={path}";

                    MPPlayer.YouTubeDlVideoQuality = CustomFunction
                        .GetYTQuality(Properties.Settings.Default.MpvNetLibYTQualityIndex);
                }
                else if (path.Contains("youtube") ||
                    path.Contains("youtu.be"))
                {
                    // YouTube 網址格式來源。
                    // Source: https://gist.github.com/rodrigoborgesdeoliveira/987683cfbfcc8d800192da1e73adc486
                    // Author: rodrigoborgesdeoliveira

                    MPPlayer.YouTubeDlVideoQuality = CustomFunction
                        .GetYTQuality(Properties.Settings.Default.MpvNetLibYTQualityIndex);
                }
                else
                {
                    // 非 YouTube 網站的影片套用此設定。 
                    MPPlayer.API.SetPropertyString("ytdl-format", "bv*+ba/b");
                }

                MPPlayer.Load(path);
                MPPlayer.Resume();

                string lyricFilePath = string.Empty;

                // 判斷 clipData.SubtitleFileUrl 是否為空值或 null
                // 且是否有啟用自動歌詞。
                if (string.IsNullOrEmpty(clipData.SubtitleFileUrl) &&
                    Properties.Settings.Default.NetPlaylistAutoLyric)
                {
                    string[] lyricData = await GetLrcFileUrl(
                        clipData.VideoUrlOrID ?? string.Empty,
                        clipData.Name ?? string.Empty,
                        clipData.StartTime.TotalSeconds.ToString());

                    string lyricFileUrl = lyricData[0],
                        offsetSeconds = lyricData[1];

                    // 轉換成正體中文。
                    bool translateToTChinese = Properties.Settings.Default.OpenCCS2TWP;

                    // 取得處理過 *.lrc 檔案的路徑。
                    lyricFilePath = await LyricsUtil.GetProcessedLrcFilePath(
                        lyricFileUrl,
                        translateToTChinese);

                    // *.lrc 檔案的網址。
                    clipData.SubtitleFileUrl = lyricFileUrl;

                    // 設定字幕檔的延遲秒數。
                    MPPlayer?.API.SetPropertyString("sub-delay", offsetSeconds);
                }
                else
                {
                    MPPlayer?.API.SetPropertyString("sub-delay", "0");
                }

                // 延後 3 秒後才執行載入字幕檔。
                await Task.Delay(3000).ContinueWith(t =>
                {
                    try
                    {
                        // 當 clipData.SubtitleFileUrl 不為空值或 null 時才載入字幕檔。
                        if (!string.IsNullOrEmpty(clipData.SubtitleFileUrl))
                        {
                            // 判斷是載入處理過的 *.lrc 檔案還是原始的 *.lrc 檔案。
                            string subtitlePath = !string.IsNullOrEmpty(lyricFilePath) ?
                                lyricFilePath :
                                clipData.SubtitleFileUrl;

                            MPPlayer?.API.Command(
                            [
                                "sub-add",
                                subtitlePath
                            ]);
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
                });
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
    /// 暫停或恢復播放短片
    /// </summary>
    private void PauseOrResumeClip()
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string message = string.Empty;

                switch (CPPlayer.Status)
                {
                    case ClipPlayerStatus.Playing:

                        MPPlayer?.Pause();

                        UpdateClipPlayer(ClipPlayerStatus.Paused, CPPlayer.ClipData);

                        BtnPause.Content = MsgSet.Resume;

                        TaskbarIconUtil.UpdateMIPauseHeader(true);

                        message = MsgSet.MsgClipPaused;

                        break;
                    case ClipPlayerStatus.Paused:
                        MPPlayer?.Resume();

                        UpdateClipPlayer(ClipPlayerStatus.Playing, CPPlayer.ClipData);

                        BtnPause.Content = MsgSet.Pause;

                        TaskbarIconUtil.UpdateMIPauseHeader(false);

                        message = MsgSet.MsgClipResumed;

                        break;
                    default:
                        break;
                }

                TaskbarIconUtil.ShowNotify(
                    message,
                    NotificationIcon.Info);
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
    /// 停止播放短片
    /// </summary>
    private void StopClip()
    {
        try
        {
            MPPlayer?.Stop();

            UpdateClipPlayer();
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
    /// 靜音短片
    /// </summary>
    /// <param name="isMuted">布林值，靜音短片，預設值為 false</param>
    private void MuteClip(bool isMuted = false)
    {
        try
        {
            if (MPPlayer != null)
            {
                MPPlayer.Volume = isMuted ? 0 : Properties.Settings.Default.MpvNetLibVolume;

                BtnMute.Content = isMuted ? MsgSet.Unmute : MsgSet.Mute;

                // 更新 TaskbarIcon 的 MIMute 的 Header。
                TaskbarIconUtil.UpdateMIMuteHeader(BtnMute.Content.ToString());

                Control[] ctrlSet =
                [
                    SVolume,
                    LVolume
                ];

                CustomFunction.BatchSetEnabled(ctrlSet, !isMuted);

                string message = isMuted ? MsgSet.MsgClipMuted : MsgSet.MsgClipMuted;

                TaskbarIconUtil.ShowNotify(
                    message,
                    NotificationIcon.Info);

                WriteLog(message: message);
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
    /// 不顯示影像
    /// </summary>
    /// <param name="enable">布林值，不顯示影像，預設值為 false</param>
    private void ClipNoVideo(bool enable = false)
    {
        try
        {
            Dispatcher.BeginInvoke(() =>
            {
                // 2023/10/3 當設定是非啟用時，才可以被短片清單中的短片設定值影響。
                // CBNoVideo 是總控制開關。
                if (!Properties.Settings.Default.MpvNetLibNoVideo)
                {
                    CBNoVideo.IsChecked = enable;
                }
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
    /// 更新 ClipPlayer
    /// </summary>
    /// <param name="clipPlayerStatus">ClipPlayerStatus</param>
    /// <param name="clipData">ClipData</param>
    private void UpdateClipPlayer(
        ClipPlayerStatus clipPlayerStatus = ClipPlayerStatus.Idle,
        ClipData? clipData = null)
    {
        try
        {
            CPPlayer.Status = clipPlayerStatus;

            if (CPPlayer.ClipData != null)
            {
                CPPlayer.ClipData.PropertyChanged -= ClipData_PropertyChanged;
            }

            CPPlayer.ClipData = clipData;

            if (CPPlayer.ClipData != null)
            {
                CPPlayer.ClipData.PropertyChanged += ClipData_PropertyChanged;
            }

            // 取得 DataGrid 的排序狀態。
            SortState sortState = GetSortState();

            #region 設定索引值相關屬性

            CPPlayer.Index = GetActualIndex(clipData);

            int tempPreIndex = CPPlayer.Index,
                tempNexIndex = CPPlayer.Index,
                previousIndex = --tempPreIndex,
                nextIndex = ++tempNexIndex;

            if (previousIndex < 0)
            {
                previousIndex = -1;
            }

            if (nextIndex > GlobalDataSet.Count - 1)
            {
                nextIndex = -1;
            }

            CPPlayer.PreviousIndex = sortState.SortDirection switch
            {
                ListSortDirection.Ascending => previousIndex,
                ListSortDirection.Descending => nextIndex,
                _ => previousIndex
            };
            CPPlayer.NextIndex = sortState.SortDirection switch
            {
                ListSortDirection.Ascending => nextIndex,
                ListSortDirection.Descending => previousIndex,
                _ => nextIndex
            };

            #endregion

            CPPlayer.SeekStatus = SSeekStatus.Idle;

            DGClipList.SelectedItem = CPPlayer.ClipData;
            DGClipList.ScrollToSelectedItem();

            if (clipData != null)
            {
                if (CPPlayer.Status != ClipPlayerStatus.Paused)
                {
                    StringInfo siClipTitle = new(clipData.Name ?? string.Empty);

                    string clipTitle = string.IsNullOrEmpty(clipData.Name) ?
                        MsgSet.ClipTitle :
                        (siClipTitle.LengthInTextElements > 40 ?
                            $"{siClipTitle.SubstringByTextElements(0, 40)}{MsgSet.Ellipses}" :
                            clipData.Name);

                    LClipTitle.Content = clipTitle;
                    LClipTitle.ToolTip = clipTitle != MsgSet.ClipTitle ?
                        clipData.Name :
                        MsgSet.ClipTitleToolTip;

                    int minVal = Convert.ToInt32(clipData.StartTime.TotalSeconds),
                        maxVal = Convert.ToInt32(clipData.EndTime.TotalSeconds);

                    PBDuration.Minimum = minVal;
                    PBDuration.Maximum = maxVal;
                    PBDuration.Value = 0;

                    SSeek.Minimum = minVal;
                    SSeek.Maximum = maxVal;
                    SSeek.Value = 0;

                    ClipNoVideo(clipData.IsAudioOnly);
                }

                if (CPPlayer.Status == ClipPlayerStatus.Playing)
                {
                    string message = string.IsNullOrEmpty(clipData.Name) ?
                        MsgSet.MsgPlayingClip :
                        MsgSet.GetFmtStr(
                            MsgSet.MsgNowPlaying,
                            clipData.Name);

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Info);
                    TaskbarIconUtil.SetToolTip(message);

                    if (Properties.Settings.Default.DiscordRichPresence)
                    {
                        string details = string.IsNullOrEmpty(clipData?.Name) ?
                            MsgSet.MsgPlayingClip :
                            clipData.Name;

                        DiscordRichPresenceUtil.SetRichPresence(
                            details: details,
                            state: MsgSet.StatePlaying,
                            timestamps: DiscordRichPresenceUtil.GetTimestamps(),
                            assets: AssetsSet.AssetsPlay);
                    }
                }
            }
            else
            {
                LClipTitle.Content = MsgSet.ClipTitle;
                LClipTitle.ToolTip = MsgSet.ClipTitleToolTip;
                LDuration.Content = "00:00:00 / 00:00:00";

                PBDuration.Minimum = 0;
                PBDuration.Maximum = 100;
                PBDuration.Value = 0;

                SSeek.Minimum = 0;
                SSeek.Maximum = 100;
                SSeek.Value = 0;

                CBPlaybackSpeed.SelectedIndex = 4;

                // 清除 TaskbarIcon 的工具提示文字。
                TaskbarIconUtil.SetToolTip(string.Empty);
            }

            switch (CPPlayer.Status)
            {
                case ClipPlayerStatus.Playing:
                    SetClipPlayerButtons(false);

                    break;
                case ClipPlayerStatus.Paused:
                    if (Properties.Settings.Default.DiscordRichPresence)
                    {
                        DiscordRichPresenceUtil.SetRichPresence(
                            state: MsgSet.StatePause,
                            timestamps: DiscordRichPresenceUtil.GetTimestamps(),
                            assets: AssetsSet.AssetsPause);
                    }

                    break;
                case ClipPlayerStatus.Idle:
                    SetClipPlayerButtons(true);

                    if (Properties.Settings.Default.DiscordRichPresence)
                    {
                        DiscordRichPresenceUtil.SetRichPresence(
                            state: MsgSet.StateStop,
                            timestamps: DiscordRichPresenceUtil.GetTimestamps(),
                            assets: AssetsSet.AssetsStop);
                    }

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

    /// <summary>
    /// 設定短片播放器的按鈕
    /// </summary>
    /// <param name="enable">布林值，啟用，預設值為 true</param>
    private void SetClipPlayerButtons(bool enable = true)
    {
        try
        {
            Control[] ctrlSet1 =
            [
                BtnPlay
            ];

            Control[] ctrlSet2 =
            [
                BtnPrevious,
                BtnNext,
                BtnPause,
                BtnStop
            ];

            CustomFunction.BatchSetEnabled(ctrlSet1, enable);
            CustomFunction.BatchSetEnabled(ctrlSet2, !enable);

            TaskbarIconUtil.SetMenuItems(enable);
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
    /// 執行隨機播放短片
    /// </summary>
    private void DoRandomPlayClip()
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (GlobalDataSet.Count <= 0)
                {
                    string message = MsgSet.MsgRandomPlayFailedClipListNoData;

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Warning);

                    ShowMsgBox(message);

                    return;
                }
                else
                {
                    int randomIndex = RandomNumberGenerator.GetInt32(0, GlobalDataSet.Count - 1);

                    StopClip();

                    ClipData clipData = GlobalDataSet[randomIndex];

                    PlayClip(clipData);
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

    /// <summary>
    /// 更新 MpvPlayer 的 LogLevel
    /// </summary>
    /// <param name="value">布林值，是否要將 mpv 的 LogLevel 設為 V 預設值是 false</param>
    private void SetMpvPlayerLogLevel(bool value = false)
    {
        try
        {
            if (MPPlayer != null)
            {
                MPPlayer.LogLevel = value ? MpvLogLevel.V : MpvLogLevel.Info;
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
    /// 取得設定的 MpvLogLevel
    /// </summary>
    /// <returns>MpvLogLevel</returns>
    private static MpvLogLevel GetMpvLogLeve()
    {
        return Properties.Settings.Default.MpvNetLibLogVerbose ? MpvLogLevel.V : MpvLogLevel.Info;
    }
}