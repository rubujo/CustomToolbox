using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Sets;
using H.NotifyIcon.Core;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CustomToolbox.Common.Utils;

/// <summary>
/// TaskbarIcon 工具
/// </summary>
public class TaskbarIconUtil
{
    /// <summary>
    /// 標題
    /// </summary>
    private static string _Title = string.Empty;

    /// <summary>
    /// TrayIconWithContextMenu
    /// </summary>
    private static TrayIconWithContextMenu? _TrayIconWithContextMenu = null;

    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

    /// <summary>
    /// 顯示／隱藏
    /// </summary>
    private static PopupMenuItem PMIShowOrHide = new();

    /// <summary>
    /// 靜音／取消靜音
    /// </summary>
    private static PopupMenuItem PMIMute = new();

    /// <summary>
    /// 不顯示影片
    /// </summary>
    private static PopupMenuItem PMINoVideo = new();

    /// <summary>
    /// 隨機播放短片
    /// </summary>
    private static PopupMenuItem PMIRandomPlayClip = new();

    /// <summary>
    /// 播放
    /// </summary>
    private static PopupMenuItem PMIPlayClip = new();

    /// <summary>
    /// 暫停
    /// </summary>
    private static PopupMenuItem PMIPause = new();

    /// <summary>
    /// 上一個
    /// </summary>
    private static PopupMenuItem PMIPrevious = new();

    /// <summary>
    /// 下一個
    /// </summary>
    private static PopupMenuItem PMINext = new();

    /// <summary>
    /// 停止
    /// </summary>
    private static PopupMenuItem PMIStop = new();

    /// <summary>
    /// 資料夾選單
    /// </summary>
    private static PopupSubMenu PSMFoldersMenu = new();

    /// <summary>
    /// 開啟 Bins 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenBinsFolder = new();

    /// <summary>
    /// 開啟設定檔資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenConfigFolder = new();

    /// <summary>
    /// 開啟 Downloads 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenDownloadsFolder = new();

    /// <summary>
    /// 開啟 ClipLists 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenCliplistsFolder = new();

    /// <summary>
    /// 開啟 Logs 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenLogsFolder = new();

    /// <summary>
    /// 開啟 Lyrics 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenLyricsFolder = new();

    /// <summary>
    /// 開啟 Temp 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenTempFolder = new();

    /// <summary>
    /// 開啟 Models 資料夾
    /// </summary>
    private static PopupMenuItem PMIOpenModelsFolder = new();

    /// <summary>
    /// 關於選單
    /// </summary>
    private static PopupSubMenu PSMAboutMenu = new();

    /// <summary>
    /// 關於
    /// </summary>
    private static PopupMenuItem PMIAbout = new();

    /// <summary>
    /// 檢查更新
    /// </summary>
    private static PopupMenuItem PMICheckUpdate = new();

    /// <summary>
    /// 結束
    /// </summary>
    private static PopupMenuItem PMIExit = new();

    /// <summary>
    /// 右鍵選單
    /// </summary>
    private static PopupMenu PMContextMenu = new();

    /// <summary>
    /// 初始化 TrayIconWithContextMenu
    /// </summary>
    /// <param name="wMain">WMain</param>
    public static void Init(WMain wMain)
    {
        try
        {
            _WMain = wMain;
            _Title = _WMain.Title;

            _TrayIconWithContextMenu = new TrayIconWithContextMenu()
            {
                Icon = Properties.Resources.app_icon.Handle,
                ToolTip = _Title
            };
            _TrayIconWithContextMenu.MessageWindow.MouseEventReceived += MessageWindow_MouseEventReceived;

            // 設定 MenuItem。
            PMIShowOrHide = new()
            {
                Text = MsgSet.Hide
            };
            PMIMute = new()
            {
                Text = _WMain.BtnMute.Content?.ToString() ?? string.Empty
            };
            PMINoVideo = new()
            {
                Text = MsgSet.MIEnableNoVideo
            };
            PMIRandomPlayClip = new()
            {
                Text = _WMain.MIRandomPlayClip.Header?.ToString() ?? string.Empty
            };
            PMIPlayClip = new()
            {
                Text = _WMain.BtnPlay.Content?.ToString() ?? string.Empty
            };
            PMIPause = new()
            {
                Text = _WMain.BtnPause.Content?.ToString() ?? string.Empty
            };
            PMIPrevious = new()
            {
                Text = _WMain.BtnPrevious.Content?.ToString() ?? string.Empty
            };
            PMINext = new()
            {
                Text = _WMain.BtnNext.Content?.ToString() ?? string.Empty
            };
            PMIStop = new()
            {
                Text = _WMain.BtnStop.Content?.ToString() ?? string.Empty
            };
            PSMFoldersMenu = new()
            {
                Text = _WMain.MIFoldersMenu.Header?.ToString() ?? string.Empty
            };
            PMIOpenBinsFolder = new()
            {
                Text = _WMain.MIOpenBinsFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenConfigFolder = new()
            {
                Text = _WMain.MIOpenConfigFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenDownloadsFolder = new()
            {
                Text = _WMain.MIOpenDownloadsFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenCliplistsFolder = new()
            {
                Text = _WMain.MIOpenCliplistsFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenLogsFolder = new()
            {
                Text = _WMain.MIOpenLogsFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenLyricsFolder = new()
            {
                Text = _WMain.MIOpenLyricsFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenTempFolder = new()
            {
                Text = _WMain.MIOpenTempFolder.Header?.ToString() ?? string.Empty
            };
            PMIOpenModelsFolder = new()
            {
                Text = _WMain.MIOpenModelsFolder.Header?.ToString() ?? string.Empty
            };
            PSMAboutMenu = new()
            {
                Text = _WMain.MIAbout.Header?.ToString() ?? string.Empty
            };
            PMICheckUpdate = new()
            {
                Text = _WMain.MICheckUpdate.Header?.ToString() ?? string.Empty
            };
            PMIAbout = new()
            {
                Text = _WMain.MIAbout.Header?.ToString() ?? string.Empty
            };
            PMIExit = new()
            {
                Text = _WMain.MIExit.Header?.ToString() ?? string.Empty
            };

            // 設定 MenuItem 的點擊事件。
            PMIShowOrHide.Click += PMIShowOrHide_Click;
            PMIMute.Click += PMIMute_Click;
            PMINoVideo.Click += PMINoVideo_Click;
            PMIRandomPlayClip.Click += PMIRandomPlayClip_Click;
            PMIPlayClip.Click += PMIPlayClip_Click;
            PMIPause.Click += PMIPause_Click;
            PMIPrevious.Click += PMIPrevious_Click;
            PMINext.Click += PMINext_Click;
            PMIStop.Click += PMIStop_Click;
            PMIOpenBinsFolder.Click += PMIOpenBinsFolder_Click;
            PMIOpenConfigFolder.Click += PMIOpenConfigFolder_Click;
            PMIOpenDownloadsFolder.Click += PMIOpenDownloadsFolder_Click;
            PMIOpenCliplistsFolder.Click += PMIOpenCliplistsFolder_Click;
            PMIOpenLogsFolder.Click += PMIOpenLogsFolder_Click;
            PMIOpenLyricsFolder.Click += PMIOpenLyricsFolder_Click;
            PMIOpenTempFolder.Click += PMIOpenTempFolder_Click;
            PMIOpenModelsFolder.Click += PMIOpenModelsFolder_Click;
            PMICheckUpdate.Click += PMICheckUpdate_Click;
            PMIAbout.Click += PMIAbout_Click;
            PMIExit.Click += PMIExit_Click;

            // 建立右鍵選單。
            PMContextMenu = new();

            PMContextMenu.Items.Clear();
            PMContextMenu.Items.Add(PMIShowOrHide);
            PMContextMenu.Items.Add(new PopupMenuSeparator());
            PMContextMenu.Items.Add(PMIMute);
            PMContextMenu.Items.Add(PMINoVideo);
            PMContextMenu.Items.Add(PMIRandomPlayClip);
            PMContextMenu.Items.Add(new PopupMenuSeparator());
            PMContextMenu.Items.Add(PMIPlayClip);
            PMContextMenu.Items.Add(PMIPause);
            PMContextMenu.Items.Add(PMIPrevious);
            PMContextMenu.Items.Add(PMINext);
            PMContextMenu.Items.Add(PMIStop);
            PMContextMenu.Items.Add(new PopupMenuSeparator());

            PSMFoldersMenu.Items.Clear();
            PSMFoldersMenu.Items.Add(PMIOpenBinsFolder);
            PSMFoldersMenu.Items.Add(PMIOpenConfigFolder);
            PSMFoldersMenu.Items.Add(PMIOpenDownloadsFolder);
            PSMFoldersMenu.Items.Add(PMIOpenCliplistsFolder);
            PSMFoldersMenu.Items.Add(PMIOpenLogsFolder);
            PSMFoldersMenu.Items.Add(PMIOpenLyricsFolder);
            PSMFoldersMenu.Items.Add(PMIOpenTempFolder);
            PSMFoldersMenu.Items.Add(PMIOpenModelsFolder);

            PMContextMenu.Items.Add(PSMFoldersMenu);
            PMContextMenu.Items.Add(new PopupMenuSeparator());

            PSMAboutMenu.Items.Clear();
            PSMAboutMenu.Items.Add(PMICheckUpdate);
            PSMAboutMenu.Items.Add(PMIAbout);

            PMContextMenu.Items.Add(PSMAboutMenu);
            PMContextMenu.Items.Add(PMIExit);

            // 設定 _TrayIconWithContextMenu 的右鍵選單。
            _TrayIconWithContextMenu.ContextMenu = PMContextMenu;

            // 設定 MenuItem 啟用／禁用。
            SetMenuItems();

            // 建立 _TrayIconWithContextMenu。
            _TrayIconWithContextMenu.Create();
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
    /// 顯示提示訊息
    /// </summary>
    /// <param name="message">字串，訊息</param>
    /// <param name="notificationIcon">NotificationIcon，預設值為 NotificationIcon.None</param>
    public static void ShowNotify(
        string message,
        NotificationIcon notificationIcon = NotificationIcon.None)
    {
        try
        {
            _TrayIconWithContextMenu?.ShowNotification(
                 _Title,
                message,
                notificationIcon);
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
    /// 設定 TaskbarIcon 的工具提示文字
    /// </summary>
    /// <param name="message">字串，訊息</param>
    public static void SetToolTip(string? message)
    {
        try
        {
            if (_TrayIconWithContextMenu != null)
            {
                _TrayIconWithContextMenu.ToolTip = string.IsNullOrEmpty(message) ? _Title : message;
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
    /// 拋棄 TrayIconWithContextMenu
    /// </summary>
    public static void Dispose()
    {
        try
        {
            if (_WMain != null)
            {
                PMIShowOrHide.Click -= PMIShowOrHide_Click;
                PMIMute.Click -= PMIMute_Click;
                PMINoVideo.Click -= PMINoVideo_Click;
                PMIRandomPlayClip.Click -= PMIRandomPlayClip_Click;
                PMIPlayClip.Click -= PMIPlayClip_Click;
                PMIPause.Click -= PMIPause_Click;
                PMIPrevious.Click -= PMIPrevious_Click;
                PMINext.Click -= PMINext_Click;
                PMIStop.Click -= PMIStop_Click;
                PMIOpenBinsFolder.Click -= PMIOpenBinsFolder_Click;
                PMIOpenConfigFolder.Click -= PMIOpenConfigFolder_Click;
                PMIOpenDownloadsFolder.Click -= PMIOpenDownloadsFolder_Click;
                PMIOpenCliplistsFolder.Click -= PMIOpenCliplistsFolder_Click;
                PMIOpenLogsFolder.Click -= PMIOpenLogsFolder_Click;
                PMIOpenLyricsFolder.Click -= PMIOpenLyricsFolder_Click;
                PMIOpenTempFolder.Click -= PMIOpenTempFolder_Click;
                PMIOpenModelsFolder.Click -= PMIOpenModelsFolder_Click;
                PMICheckUpdate.Click -= PMICheckUpdate_Click;
                PMIAbout.Click -= PMIAbout_Click;
                PMIExit.Click -= PMIExit_Click;
            }

            if (_TrayIconWithContextMenu != null)
            {
                _TrayIconWithContextMenu.Dispose();
                _TrayIconWithContextMenu = null;
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
    /// 設定 MenuItem 啟用／禁用
    /// </summary>
    /// <param name="enable">布林值，啟用，預設值為 true</param>
    public static void SetMenuItems(bool enable = true)
    {
        try
        {
            PopupMenuItem[] ctrlSet1 =
            [
                PMIPlayClip
            ];

            PopupMenuItem[] ctrlSet2 =
            [
                PMIPrevious,
                PMINext,
                PMIPause,
                PMIStop
            ];

            CustomFunction.BatchSetEnabled(ctrlSet1, enable);
            CustomFunction.BatchSetEnabled(ctrlSet2, !enable);
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
    /// 更新 MIMute 的 Header
    /// </summary>
    /// <param name="value">字串，值</param>
    public static void UpdateMIMuteHeader(string? value)
    {
        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                PMIMute.Text = value;
            }
            else
            {
                PMIMute.Text = _WMain?.BtnMute.Content?.ToString() ?? string.Empty;
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
    /// 更新 MIShowOrHide 的 Header
    /// </summary>
    /// <param name="visibility">Visibility</param>
    public static void UpdateMIShowOrHideHeader(Visibility visibility)
    {
        try
        {
            switch (visibility)
            {
                case Visibility.Visible:
                    PMIShowOrHide.Text = MsgSet.Hide;

                    break;
                case Visibility.Collapsed:
                    PMIShowOrHide.Text = MsgSet.Show;

                    break;
                case Visibility.Hidden:
                    PMIShowOrHide.Text = MsgSet.Show;

                    break;
                default:
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
    /// 更新 MINoVideo 的 Header
    /// </summary>
    /// <param name="isChecked">布林值，預設值為 false</param>
    public static void UpdateMINoVideoHeader(bool isChecked = false)
    {
        try
        {
            string header = isChecked == true ?
                MsgSet.MIDisableNoVideo :
                MsgSet.MIEnableNoVideo;

            PMINoVideo.Text = header;
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
    /// 更新 MIPause 的 Header
    /// </summary>
    /// <param name="isPaused">布林值，預設值為 false</param>
    public static void UpdateMIPauseHeader(bool isPaused = false)
    {
        try
        {
            string header = isPaused == true ?
                MsgSet.Resume :
                MsgSet.Pause;

            PMIPause.Text = header;
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

    #region 事件

    private static void MessageWindow_MouseEventReceived(object? sender, MessageWindow.MouseEventReceivedEventArgs e)
    {
        try
        {
            if (e.MouseEvent == MouseEvent.IconLeftDoubleClick)
            {
                PMIShowOrHide_Click(sender, e);
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

    private static void PMIShowOrHide_Click(object? sender, EventArgs e)
    {
        try
        {
            Visibility visibility = CustomFunction.ShowOrHideWindow(_WMain);

            UpdateMIShowOrHideHeader(visibility);
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

    private static void PMINoVideo_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_WMain != null)
                {
                    if (_WMain.CBNoVideo.IsChecked == true)
                    {
                        _WMain.CBNoVideo.IsChecked = false;
                    }
                    else
                    {
                        _WMain.CBNoVideo.IsChecked = true;
                    }

                    string header = _WMain.CBNoVideo.IsChecked == true ?
                        MsgSet.MIDisableNoVideo :
                        MsgSet.MIEnableNoVideo;

                    PMINoVideo.Text = header;
                }
            }));
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

    private static void PMIMute_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnMute_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIRandomPlayClip_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIRandomPlayClip_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIPlayClip_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnPlay_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIPause_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnPause_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIPrevious_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnPrevious_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMINext_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnNext_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIStop_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.BtnStop_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMICheckUpdate_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MICheckUpdate_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenBinsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenBinsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenConfigFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenConfigFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenDownloadsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenDownloadsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenCliplistsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenCliplistsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenLogsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenLogsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenLyricsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenLyricsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenTempFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenTempFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIOpenModelsFolder_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIOpenModelsFolder_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIAbout_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIAbout_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    private static void PMIExit_Click(object? sender, EventArgs e)
    {
        try
        {
            _WMain?.MIExit_Click(sender, new RoutedEventArgs(MenuItem.ClickEvent));
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

    #endregion
}