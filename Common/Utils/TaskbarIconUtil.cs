﻿using Control = System.Windows.Controls.Control;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Sets;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;

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
    /// TaskbarIcon
    /// </summary>
    private static TaskbarIcon? _TaskbarIcon = null;

    /// <summary>
    /// WMain
    /// </summary>
    private static WMain? _WMain = null;

    /// <summary>
    /// 顯示／隱藏
    /// </summary>
    private static MenuItem MIShowOrHide = new();

    /// <summary>
    /// 靜音／取消靜音
    /// </summary>
    private static MenuItem MIMute = new();

    /// <summary>
    /// 不顯示影片
    /// </summary>
    private static MenuItem MINoVideo = new();

    /// <summary>
    /// 隨機播放短片
    /// </summary>
    private static MenuItem MIRandomPlayClip = new();

    /// <summary>
    /// 播放
    /// </summary>
    private static MenuItem MIPlayClip = new();

    /// <summary>
    /// 暫停
    /// </summary>
    private static MenuItem MIPause = new();

    /// <summary>
    /// 上一個
    /// </summary>
    private static MenuItem MIPrevious = new();

    /// <summary>
    /// 下一個
    /// </summary>
    private static MenuItem MINext = new();

    /// <summary>
    /// 停止
    /// </summary>
    private static MenuItem MIStop = new();

    /// <summary>
    /// 資料夾選單
    /// </summary>
    private static MenuItem MIFoldersMenu = new();

    /// <summary>
    /// 開啟 Bins 資料夾
    /// </summary>
    private static MenuItem MIOpenBinsFolder = new();

    /// <summary>
    /// 開啟設定檔資料夾
    /// </summary>
    private static MenuItem MIOpenConfigFolder = new();

    /// <summary>
    /// 開啟 Downloads 資料夾
    /// </summary>
    private static MenuItem MIOpenDownloadsFolder = new();

    /// <summary>
    /// 開啟 ClipLists 資料夾
    /// </summary>
    private static MenuItem MIOpenCliplistsFolder = new();

    /// <summary>
    /// 開啟 Logs 資料夾
    /// </summary>
    private static MenuItem MIOpenLogsFolder = new();

    /// <summary>
    /// 開啟 Lyrics 資料夾
    /// </summary>
    private static MenuItem MIOpenLyricsFolder = new();

    /// <summary>
    /// 開啟 Temp 資料夾
    /// </summary>
    private static MenuItem MIOpenTempFolder = new();

    /// <summary>
    /// 開啟 Models 資料夾
    /// </summary>
    private static MenuItem MIOpenModelsFolder = new();

    /// <summary>
    /// 關於選單
    /// </summary>
    private static MenuItem MIAboutMenu = new();

    /// <summary>
    /// 關於
    /// </summary>
    private static MenuItem MIAbout = new();

    /// <summary>
    /// 檢查更新
    /// </summary>
    private static MenuItem MICheckUpdate = new();

    /// <summary>
    /// 結束
    /// </summary>
    private static MenuItem MIExit = new();

    /// <summary>
    /// 右鍵選單
    /// </summary>
    private static ContextMenu CMContextMenu = new();

    /// <summary>
    /// 初始化 TaskbarIcon
    /// </summary>
    /// <param name="wMain">WMain</param>
    /// <param name="taskbarIcon">TaskbarIcon</param>
    public static void Init(WMain wMain, TaskbarIcon taskbarIcon)
    {
        try
        {
            _TaskbarIcon = taskbarIcon;
            _WMain = wMain;
            _Title = _WMain.Title;

            _TaskbarIcon.Icon = Properties.Resources.app_icon;
            _TaskbarIcon.ToolTipText = _Title;

            // 設定 _TaskbarIcon 的滑鼠雙點擊事件。
            _TaskbarIcon.TrayMouseDoubleClick += TaskbarIcon_TrayMouseDoubleClick;

            // 設定 MenuItem。
            MIShowOrHide = new()
            {
                Header = MsgSet.Hide
            };
            MIMute = new()
            {
                Header = _WMain.BtnMute.Content
            };
            MINoVideo = new()
            {
                Header = MsgSet.MIEnableNoVideo
            };
            MIRandomPlayClip = new()
            {
                Header = _WMain.MIRandomPlayClip.Header
            };
            MIPlayClip = new()
            {
                Header = _WMain.BtnPlay.Content
            };
            MIPause = new()
            {
                Header = _WMain.BtnPause.Content
            };
            MIPrevious = new()
            {
                Header = _WMain.BtnPrevious.Content
            };
            MINext = new()
            {
                Header = _WMain.BtnNext.Content
            };
            MIStop = new()
            {
                Header = _WMain.BtnStop.Content
            };
            MIFoldersMenu = new()
            {
                Header = _WMain.MIFoldersMenu.Header
            };
            MIOpenBinsFolder = new()
            {
                Header = _WMain.MIOpenBinsFolder.Header
            };
            MIOpenConfigFolder = new()
            {
                Header = _WMain.MIOpenConfigFolder.Header
            };
            MIOpenDownloadsFolder = new()
            {
                Header = _WMain.MIOpenDownloadsFolder.Header
            };
            MIOpenCliplistsFolder = new()
            {
                Header = _WMain.MIOpenCliplistsFolder.Header
            };
            MIOpenLogsFolder = new()
            {
                Header = _WMain.MIOpenLogsFolder.Header
            };
            MIOpenLyricsFolder = new()
            {
                Header = _WMain.MIOpenLyricsFolder.Header
            };
            MIOpenTempFolder = new()
            {
                Header = _WMain.MIOpenTempFolder.Header
            };
            MIOpenModelsFolder = new()
            {
                Header = _WMain.MIOpenModelsFolder.Header
            };
            MIAboutMenu = new()
            {
                Header = _WMain.MIAbout.Header
            };
            MICheckUpdate = new()
            {
                Header = _WMain.MICheckUpdate.Header
            };
            MIAbout = new()
            {
                Header = _WMain.MIAbout.Header
            };
            MIExit = new()
            {
                Header = _WMain.MIExit.Header
            };

            // 設定 MenuItem 的點擊事件。
            MIShowOrHide.Click += MIShowOrHide_Click;
            MIMute.Click += _WMain.BtnMute_Click;
            MINoVideo.Click += MINoVideo_Click;
            MIRandomPlayClip.Click += _WMain.MIRandomPlayClip_Click;
            MIPlayClip.Click += _WMain.BtnPlay_Click;
            MIPause.Click += _WMain.BtnPause_Click;
            MIPrevious.Click += _WMain.BtnPrevious_Click;
            MINext.Click += _WMain.BtnNext_Click;
            MIStop.Click += _WMain.BtnStop_Click;
            MICheckUpdate.Click += _WMain.MICheckUpdate_Click;
            MIOpenBinsFolder.Click += _WMain.MIOpenBinsFolder_Click;
            MIOpenConfigFolder.Click += _WMain.MIOpenConfigFolder_Click;
            MIOpenDownloadsFolder.Click += _WMain.MIOpenDownloadsFolder_Click;
            MIOpenCliplistsFolder.Click += _WMain.MIOpenCliplistsFolder_Click;
            MIOpenLogsFolder.Click += _WMain.MIOpenLogsFolder_Click;
            MIOpenLyricsFolder.Click += _WMain.MIOpenLyricsFolder_Click;
            MIOpenTempFolder.Click += _WMain.MIOpenTempFolder_Click;
            MIOpenModelsFolder.Click += _WMain.MIOpenModelsFolder_Click;
            MIAbout.Click += _WMain.MIAbout_Click;
            MIExit.Click += _WMain.MIExit_Click;

            // 建立右鍵選單。
            CMContextMenu = new();

            CMContextMenu.Items.Clear();
            CMContextMenu.Items.Add(MIShowOrHide);
            CMContextMenu.Items.Add(new Separator());
            CMContextMenu.Items.Add(MIMute);
            CMContextMenu.Items.Add(MINoVideo);
            CMContextMenu.Items.Add(MIRandomPlayClip);
            CMContextMenu.Items.Add(new Separator());
            CMContextMenu.Items.Add(MIPlayClip);
            CMContextMenu.Items.Add(MIPause);
            CMContextMenu.Items.Add(MIPrevious);
            CMContextMenu.Items.Add(MINext);
            CMContextMenu.Items.Add(MIStop);
            CMContextMenu.Items.Add(new Separator());

            MIFoldersMenu.Items.Clear();
            MIFoldersMenu.Items.Add(MIOpenBinsFolder);
            MIFoldersMenu.Items.Add(MIOpenConfigFolder);
            MIFoldersMenu.Items.Add(MIOpenDownloadsFolder);
            MIFoldersMenu.Items.Add(MIOpenCliplistsFolder);
            MIFoldersMenu.Items.Add(MIOpenLogsFolder);
            MIFoldersMenu.Items.Add(MIOpenLyricsFolder);
            MIFoldersMenu.Items.Add(MIOpenTempFolder);
            MIFoldersMenu.Items.Add(MIOpenModelsFolder);

            CMContextMenu.Items.Add(MIFoldersMenu);
            CMContextMenu.Items.Add(new Separator());

            MIAboutMenu.Items.Clear();
            MIAboutMenu.Items.Add(MICheckUpdate);
            MIAboutMenu.Items.Add(MIAbout);

            CMContextMenu.Items.Add(MIAboutMenu);
            CMContextMenu.Items.Add(MIExit);

            // 設定 _TaskbarIcon 的右鍵選單。
            _TaskbarIcon.ContextMenu = CMContextMenu;

            // 設定 MenuItem 啟用／禁用。
            SetMenuItems();
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

    private static void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        try
        {
            MIShowOrHide.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
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
            _TaskbarIcon?.ShowNotification(
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
            if (_TaskbarIcon != null)
            {
                _TaskbarIcon.ToolTipText = string.IsNullOrEmpty(message) ? _Title : message;
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
    /// 拋棄 TaskbarIcon
    /// </summary>
    public static void Dispose()
    {
        try
        {
            if (_WMain != null)
            {
                MIShowOrHide.Click -= MIShowOrHide_Click;
                MIMute.Click -= _WMain.BtnMute_Click;
                MINoVideo.Click -= MINoVideo_Click;
                MIRandomPlayClip.Click -= _WMain.MIRandomPlayClip_Click;
                MIPlayClip.Click -= _WMain.BtnPlay_Click;
                MIPause.Click -= _WMain.BtnPause_Click;
                MIPrevious.Click -= _WMain.BtnPrevious_Click;
                MINext.Click -= _WMain.BtnNext_Click;
                MIStop.Click -= _WMain.BtnStop_Click;
                MIOpenBinsFolder.Click -= _WMain.MIOpenBinsFolder_Click;
                MIOpenConfigFolder.Click -= _WMain.MIOpenConfigFolder_Click;
                MIOpenDownloadsFolder.Click -= _WMain.MIOpenDownloadsFolder_Click;
                MIOpenCliplistsFolder.Click -= _WMain.MIOpenCliplistsFolder_Click;
                MIOpenLogsFolder.Click -= _WMain.MIOpenLogsFolder_Click;
                MIOpenLyricsFolder.Click -= _WMain.MIOpenLyricsFolder_Click;
                MIOpenTempFolder.Click -= _WMain.MIOpenTempFolder_Click;
                MIOpenModelsFolder.Click -= _WMain.MIOpenModelsFolder_Click;
                MICheckUpdate.Click -= _WMain.MICheckUpdate_Click;
                MIAbout.Click -= _WMain.MIAbout_Click;
                MIExit.Click -= _WMain.MIExit_Click;
            }

            if (_TaskbarIcon != null)
            {
                _TaskbarIcon.Dispose();
                _TaskbarIcon = null;
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
            Control[] ctrlSet1 =
            [
                MIPlayClip
            ];

            Control[] ctrlSet2 =
            [
                MIPrevious,
                MINext,
                MIPause,
                MIStop
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
                MIMute.Header = value;
            }
            else
            {
                MIMute.Header = _WMain?.BtnMute.Content;
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
                    MIShowOrHide.Header = MsgSet.Hide;

                    break;
                case Visibility.Collapsed:
                    MIShowOrHide.Header = MsgSet.Show;

                    break;
                case Visibility.Hidden:
                    MIShowOrHide.Header = MsgSet.Show;

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

    private static void MIShowOrHide_Click(object sender, RoutedEventArgs e)
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

    private static void MINoVideo_Click(object sender, RoutedEventArgs e)
    {
        try
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

                MINoVideo.Header = header;
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

            MINoVideo.Header = header;
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

            MIPause.Header = header;
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
}