using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using CustomToolbox.Common;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Utils;
using CustomToolbox.Common.Sets;
using static CustomToolbox.Common.Sets.EnumSet;
using DragCompletedEventArgs = System.Windows.Controls.Primitives.DragCompletedEventArgs;
using DragStartedEventArgs = System.Windows.Controls.Primitives.DragStartedEventArgs;
using H.NotifyIcon.Core;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Mpv.NET.API;
using Mpv.NET.Player;
using Serilog.Events;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomToolbox;

/// <summary>
/// TIClipPlayer 的控制項事件
/// </summary>
public partial class WMain
{
    private void ClipData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.PropertyName == nameof(ClipData.Name))
                {
                    UpdateClipPlayer(
                        CPPlayer.Status,
                        CPPlayer.ClipData);
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

    private void SSeek_KeyUpEvent(object? sender, KeyEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Key == Key.Up ||
                    e.Key == Key.Left ||
                    e.Key == Key.Down ||
                    e.Key == Key.Right)
                {
                    if (!IsPending)
                    {
                        IsPending = true;

                        // 延後 3 秒在執行。
                        Task.Delay(3000)
                            .ContinueWith(task =>
                            {
                                CPPlayer.SeekStatus = SSeekStatus.Idle;

                                IsPending = false;
                            });
                    }
                }
                else
                {
                    CPPlayer.SeekStatus = SSeekStatus.Idle;
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

    private void SSeek_KeyDownEvent(object? sender, KeyEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Key == Key.Up ||
                    e.Key == Key.Left ||
                    e.Key == Key.Down ||
                    e.Key == Key.Right)
                {
                    CPPlayer.SeekStatus = SSeekStatus.Drag;
                }
                else
                {
                    CPPlayer.SeekStatus = SSeekStatus.Idle;
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

    private void SSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Slider slider = (Slider)sender;

                double newValue = e.NewValue;

                if (MPPlayer != null && MPPlayer.IsMediaLoaded)
                {
                    if (CPPlayer.SeekStatus == SSeekStatus.Drag)
                    {
                        if (newValue >= slider.Minimum &&
                            newValue <= slider.Maximum)
                        {
                            // 在某些特定狀況下 newValue 會大於 MPPlayer.Duration.TotalSeconds，
                            // 需要排除此狀況。
                            if (newValue <= MPPlayer.Duration.TotalSeconds)
                            {
                                try
                                {
                                    MPPlayer.SeekAsync(newValue);
                                }
                                catch (ArgumentOutOfRangeException aoore)
                                {
                                    WriteLog(
                                        message: MsgSet.GetFmtStr(
                                            MsgSet.MsgErrorOccured,
                                            aoore.GetExceptionMessage()),
                                        logEventLevel: LogEventLevel.Error);
                                }
                                catch (MpvAPIException mae)
                                {
                                    WriteLog(
                                        message: MsgSet.GetFmtStr(
                                            MsgSet.MsgErrorOccured,
                                            mae.GetExceptionMessage()),
                                        logEventLevel: LogEventLevel.Error);
                                }
                            }
                        }
                    }
                }
                else
                {
                    slider.Value = slider.Minimum;
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

    private void SSeek_DragStarted(object sender, DragStartedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CPPlayer.SeekStatus = SSeekStatus.Drag;
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

    private void SSeek_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CPPlayer.SeekStatus = SSeekStatus.Idle;
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

    private void SVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int newValue = (int)e.NewValue;

                if (MPPlayer != null)
                {
                    MPPlayer.Volume = newValue;

                    if (Properties.Settings.Default.MpvNetLibVolume != newValue)
                    {
                        Properties.Settings.Default.MpvNetLibVolume = newValue;
                        Properties.Settings.Default.Save();
                    }

                    LVolume.Content = newValue.ToString();
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

    private void CBNoVideo_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox control = (CheckBox)sender;

                bool value = control.IsChecked ?? true;

                if (value)
                {
                    if (Properties.Settings.Default.MpvNetLibNoVideo != value)
                    {
                        Properties.Settings.Default.MpvNetLibNoVideo = value;
                        Properties.Settings.Default.Save();
                    }

                    TaskbarIconUtil.UpdateMINoVideoHeader(value);

                    string? vid = MPPlayer?.API.GetPropertyString("vid");

                    if (vid != "no")
                    {
                        MPPlayer?.API.SetPropertyString("vid", "no");

                        string message = MsgSet.MsgNoVideoEnabled;

                        TaskbarIconUtil.ShowNotify(
                            message,
                            NotificationIcon.Info);

                        WriteLog(message);
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

    private void CBNoVideo_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox control = (CheckBox)sender;

                bool value = control.IsChecked ?? false;

                if (!value)
                {
                    if (Properties.Settings.Default.MpvNetLibNoVideo != value)
                    {
                        Properties.Settings.Default.MpvNetLibNoVideo = value;
                        Properties.Settings.Default.Save();
                    }

                    TaskbarIconUtil.UpdateMINoVideoHeader(value);

                    string? vid = MPPlayer?.API.GetPropertyString("vid");

                    if (vid != "auto")
                    {
                        MPPlayer?.API.SetPropertyString("vid", "auto");

                        string message = MsgSet.MsgNoVideoDisabled;

                        TaskbarIconUtil.ShowNotify(
                            message,
                            NotificationIcon.Info);

                        WriteLog(message: message);
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

    private void CBChromaKey_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox control = (CheckBox)sender;

                bool value = control.IsChecked ?? true;

                if (value)
                {
                    if (Properties.Settings.Default.MpvNetLibChromaKey != value)
                    {
                        Properties.Settings.Default.MpvNetLibChromaKey = value;
                        Properties.Settings.Default.Save();
                    }

                    string? vf = MPPlayer?.API.GetPropertyString("vf");

                    if (string.IsNullOrEmpty(vf) ||
                        !vf.Contains("colorize"))
                    {
                        // 綠幕。
                        MPPlayer?.API.SetPropertyString("vf", VariableSet.VideoFilterColorize);
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

    private void CBChromaKey_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox control = (CheckBox)sender;

                bool value = control.IsChecked ?? false;

                if (!value)
                {
                    if (Properties.Settings.Default.MpvNetLibChromaKey != value)
                    {
                        Properties.Settings.Default.MpvNetLibChromaKey = value;
                        Properties.Settings.Default.Save();
                    }

                    string? vf = MPPlayer?.API.GetPropertyString("vf");

                    if (!string.IsNullOrEmpty(vf) &&
                        vf != VariableSet.VideoFilterNull)
                    {
                        MPPlayer?.API.SetPropertyString("vf", VariableSet.VideoFilterNull);
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

    private void CBYTQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ComboBox comboBox = (ComboBox)sender;

                if (comboBox != null)
                {
                    int selectedIndex = comboBox.SelectedIndex;

                    YouTubeDlVideoQuality quality = CustomFunction.GetYTQuality(selectedIndex);

                    if (MPPlayer != null)
                    {
                        if (Properties.Settings.Default.MpvNetLibYTQualityIndex != selectedIndex)
                        {
                            Properties.Settings.Default.MpvNetLibYTQualityIndex = selectedIndex;
                            Properties.Settings.Default.Save();
                        }

                        MPPlayer.YouTubeDlVideoQuality = quality;

                        // 避免在應用程式剛開始執行時就輸出提示訊息。
                        if (!IsInitializing)
                        {
                            WriteLog(MsgSet.MsgYouTubeVideoQualityChanged);
                        }
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

    private void CBSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ComboBox comboBox = (ComboBox)sender;

                if (comboBox != null)
                {
                    int selectedIndex = comboBox.SelectedIndex;

                    double speed = CustomFunction.GetPlaybackSpeed(selectedIndex);

                    if (MPPlayer != null)
                    {
                        if (Properties.Settings.Default.MpvNetLibPlaybackSpeedIndex != selectedIndex)
                        {
                            Properties.Settings.Default.MpvNetLibPlaybackSpeedIndex = selectedIndex;
                            Properties.Settings.Default.Save();
                        }

                        MPPlayer.Speed = speed;
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

    public void BtnMute_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MPPlayer != null)
                {
                    MuteClip(MPPlayer.Volume > 0);
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

    public void BtnPlay_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (GlobalDataSet.Count <= 0)
                {
                    string message = MsgSet.MsgPlayFailedClipListNoData;

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Warning);

                    ShowMsgBox(message);

                    return;
                }

                StopClip();

                int initIndex = 0;

                ClipData clipData = GetSortedGlobalDataSet()[initIndex];

                PlayClip(clipData);
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

    public void BtnPause_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(PauseOrResumeClip));
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

    public void BtnPrevious_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 取得 DGClipList 的排序狀態。
                SortState sortState = GetSortState();

                int previousIndex = sortState.SortDirection switch
                {
                    ListSortDirection.Ascending => CPPlayer.PreviousIndex,
                    ListSortDirection.Descending => CPPlayer.NextIndex,
                    _ => CPPlayer.PreviousIndex
                };

                if (previousIndex > -1)
                {
                    ClipData clipData = GetSortedGlobalDataSet()[previousIndex];

                    PlayClip(clipData);
                }
                else
                {
                    BtnStop_Click(
                        nameof(BtnPrevious),
                        new RoutedEventArgs(ButtonBase.ClickEvent));

                    string message = MsgSet.MsgNoPreviousClip;

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Info);

                    ShowMsgBox(message);
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

    public void BtnNext_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 取得 DGClipList 的排序狀態。
                SortState sortState = GetSortState();

                int nextIndex = sortState.SortDirection switch
                {
                    ListSortDirection.Ascending => CPPlayer.NextIndex,
                    ListSortDirection.Descending => CPPlayer.PreviousIndex,
                    _ => CPPlayer.NextIndex
                };

                if (nextIndex > -1)
                {
                    ClipData clipData = GetSortedGlobalDataSet()[nextIndex];

                    PlayClip(clipData);
                }
                else
                {
                    BtnStop_Click(
                        nameof(BtnNext_Click),
                        new RoutedEventArgs(ButtonBase.ClickEvent));

                    string message = MsgSet.MsgNoNextClip;

                    TaskbarIconUtil.ShowNotify(
                        message,
                        NotificationIcon.Info);

                    ShowMsgBox(message);
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

    public void BtnStop_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StopClip();

                // 當停止播放時，關閉彈出視窗。
                if (WPPPlayer != null && WPPPlayer.IsShowing())
                {
                    WPPPlayer.Close();
                    WPPPlayer = null;
                }

                TaskbarIconUtil.ShowNotify(
                    MsgSet.MsgStoppedPlayingClip,
                    NotificationIcon.Info);
                TaskbarIconUtil.SetToolTip(string.Empty);
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

    private void RBClipPlayer_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CPPlayer.Mode = RBClipPlayer.IsChecked == true ?
                    ClipPlayerMode.ClipPlayer :
                    ClipPlayerMode.TimestampEditor;

                if (RBClipPlayer.IsChecked == true)
                {
                    WriteLog(MsgSet.MsgSwitchToClipPlayerMode);
                }
                else
                {
                    WriteLog(MsgSet.MsgSwitchToTimestampEditorMode);
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

    private void RBTimestampEditor_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CPPlayer.Mode = RBTimestampEditor.IsChecked == true ?
                    ClipPlayerMode.TimestampEditor :
                    ClipPlayerMode.ClipPlayer;

                if (RBTimestampEditor.IsChecked == true)
                {
                    WriteLog(message: MsgSet.MsgSwitchToTimestampEditorMode);
                }
                else
                {
                    WriteLog(message: MsgSet.MsgSwitchToClipPlayerMode);
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
}