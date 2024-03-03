using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using CustomToolbox.Common;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Utils;
using H.NotifyIcon;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using RichTextBox = System.Windows.Controls.RichTextBox;
using Serilog.Events;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using TextBox = System.Windows.Controls.TextBox;

namespace CustomToolbox;

/// <summary>
/// Interaction logic for WMain.xaml
/// </summary>
public partial class WMain : Window
{
    public WMain(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();

        GlobalHCFactory = httpClientFactory;
    }

    private void WMain_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            CustomInit();
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

    private void WMain_Closing(object sender, CancelEventArgs e)
    {
        try
        {
            if (!IsByPassingWindowClosingEventCancel)
            {
                // 先攔截事件。
                e.Cancel = true;

                // 關閉應用程式。
                ShutdownApp(sender);
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

    public void WMain_KeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            // 排除組合按鍵。
            if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift ||
                e.Key == Key.RightShift ||
                e.Key == Key.LeftAlt ||
                e.Key == Key.RightAlt)
            {
                return;
            }

            // 避免在非指定的 RichTextBox 內輸入內容時觸發快速鍵。
            if (e.OriginalSource is RichTextBox richTextBox)
            {
                if (richTextBox != null && richTextBox != RTBLog)
                {
                    return;
                }
            }

            // 避免在 TextBox 內輸入內容時觸發快速鍵。
            if (e.OriginalSource is TextBox textBox)
            {
                if (textBox != null)
                {
                    return;
                }
            }

            switch (e.Key)
            {
                case Key.Q:
                    // 強制顯示應用程式以顯示對話視窗。
                    WindowExtensions.Show(
                        this,
                        disableEfficiencyMode: true);

                    WindowState = WindowState.Normal;

                    ShutdownApp(sender);

                    break;
                case Key.W:
                    // 重設 PopupPlayer 的大小與位置。
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (WPPPlayer != null && WPPPlayer.IsShowing())
                        {
                            WPPPlayer.Width = 854;
                            WPPPlayer.Height = 480;
                            WPPPlayer.Top = 60;
                            WPPPlayer.Left = 60;
                        }
                    }));
                    break;
                case Key.E:
                    PlayerHost_MouseDoubleClick(
                        sender,
                        new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));

                    break;
                case Key.R:
                    // 開關 PopupPlayer 的全螢幕顯示。
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (WPPPlayer != null && WPPPlayer.IsShowing())
                        {
                            if (WPPPlayer.WindowState != WindowState.Maximized)
                            {
                                WPPPlayer.WindowStyle = WindowStyle.None;
                                WPPPlayer.WindowState = WindowState.Normal;
                                WPPPlayer.WindowState = WindowState.Maximized;
                            }
                            else
                            {
                                WPPPlayer.WindowStyle = WindowStyle.SingleBorderWindow;
                                WPPPlayer.WindowState = WindowState.Normal;
                            }
                        }
                    }));

                    break;
                case Key.T:
                    // 開關 PopupPlayer 的 TopMost。
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (WPPPlayer != null && WPPPlayer.IsShowing())
                        {
                            bool isTopmost = WPPPlayer.Topmost;

                            if (isTopmost)
                            {
                                WPPPlayer.Topmost = false;
                            }
                            else
                            {
                                WPPPlayer.Topmost = true;
                            }
                        }
                    }));

                    break;
                case Key.Y:
                    Visibility visibility = CustomFunction.ShowOrHideWindow(this);

                    TaskbarIconUtil.UpdateMIShowOrHideHeader(visibility);

                    break;
                case Key.A:
                    if (BtnPlay.IsEnabled)
                    {
                        BtnPlay.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.S:
                    if (BtnPause.IsEnabled)
                    {
                        BtnPause.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.D:
                    if (BtnPrevious.IsEnabled)
                    {
                        BtnPrevious.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.F:
                    if (BtnNext.IsEnabled)
                    {
                        BtnNext.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.G:
                    if (BtnStop.IsEnabled)
                    {
                        BtnStop.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.Z:
                    DoRandomPlayClip();

                    break;
                case Key.X:
                    DoReorderClipList();

                    break;
                case Key.C:
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (CBNoVideo.IsEnabled)
                        {
                            CBNoVideo.IsChecked = !CBNoVideo.IsChecked;
                        }
                    }));

                    break;
                case Key.V:
                    if (BtnMute.IsEnabled)
                    {
                        BtnMute.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                    break;
                case Key.U:
                case Key.I:
                    {
                        if (CPPlayer.Mode != EnumSet.ClipPlayerMode.TimestampEditor)
                        {
                            ShowMsgBox(MsgSet.MsgSwitchToTimestampEditorModeFirst);

                            return;
                        }

                        ClipData? clipData = CPPlayer.ClipData;

                        if (clipData == null)
                        {
                            ShowMsgBox(MsgSet.MsgPlayAClipFirst);

                            return;
                        }

                        if (MPPlayer == null)
                        {
                            ShowMsgBox(MsgSet.MsgLibMpvIsNotLoaded);

                            return;
                        }

                        if (!MPPlayer.IsMediaLoaded)
                        {
                            ShowMsgBox(MsgSet.MsgMediaIsNotLoaded);

                            return;
                        }

                        double newSeconds = Math.Round(
                             MPPlayer.Position.TotalSeconds,
                             MidpointRounding.AwayFromZero);

                        if (e.Key == Key.U)
                        {
                            clipData.StartTime = TimeSpan.FromSeconds(newSeconds);

                            WriteLog(
                                message: MsgSet.GetFmtStr(
                                    MsgSet.TemplateUpdateStarTimeOfClipTo,
                                    clipData.Name ?? string.Empty,
                                    clipData.StartTime.ToString()));
                        }
                        else if (e.Key == Key.I)
                        {
                            clipData.EndTime = TimeSpan.FromSeconds(newSeconds);

                            WriteLog(
                                message: MsgSet.GetFmtStr(
                                    MsgSet.TemplateUpdateEndTimeOfClipTo,
                                    clipData.Name ?? string.Empty,
                                    clipData.EndTime.ToString()));
                        }
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
}