using CustomToolbox.Common;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Utils;
using Serilog.Events;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using TextBox = System.Windows.Controls.TextBox;

namespace CustomToolbox;

/// <summary>
/// TIMisc 的控制項事件
/// </summary>
public partial class WMain
{
    #region 應用程式選項

    private void CBLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    ComboBox control = (ComboBox)sender;
                    LangData selectedItem = (LangData)control.SelectedItem;

                    string value = selectedItem.LangCode ?? string.Empty,
                        langName = selectedItem.LangName ?? string.Empty;

                    // 當值與已儲存值不一樣時才觸發更語系的換行為。
                    if (Properties.Settings.Default.AppLangCode != value)
                    {
                        string message = MsgSet.GetFmtStr(
                            MsgSet.MsgChangeLanguage,
                            langName);

                        ShowConfirmMsgBox(
                            message: message,
                            primaryAction: new Action(() =>
                            {
                                IsByPassingWindowClosingEventCancel = true;

                                Properties.Settings.Default.AppLangCode = value;
                                Properties.Settings.Default.Save();

                                CustomFunction.RestartApplication(isImmediately: true);
                            }),
                            cancelAction: new Action(() =>
                            {
                                IsByPassingWindowClosingEventCancel = false;

                                // 設回預設值。
                                control.SelectedValue = Properties.Settings.Default.AppLangCode;
                            }));
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

    private void TBUserAgent_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    string value = control.Text;

                    if (Properties.Settings.Default.UserAgent != value)
                    {
                        Properties.Settings.Default.UserAgent = value;
                        Properties.Settings.Default.Save();

                        WriteLog(message: MsgSet.MsgUpdateUserAgent);
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

    private void TBSecChUa_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    string value = control.Text;

                    if (Properties.Settings.Default.SecChUa != value)
                    {
                        Properties.Settings.Default.SecChUa = value;
                        Properties.Settings.Default.Save();

                        WriteLog(message: MsgSet.MsgUpdateSecChUa);
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

    private void BtnMyUserAgent_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CustomFunction.OpenUrl(UrlSet.QueryUserAgentUrl);
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

    private void TBUnsupportedDomains_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    string[] tempValue = control.Text.Split(
                        Environment.NewLine.ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries);

                    string value = string.Join(";", tempValue);

                    if (Properties.Settings.Default.NetPlaylistUnsupportedDomains != value)
                    {
                        Properties.Settings.Default.NetPlaylistUnsupportedDomains = value;
                        Properties.Settings.Default.Save();

                        WriteLog(message: MsgSet.MsgUpdateUnsupportedDomains);

                        Task.Delay(1500).ContinueWith(t =>
                        {
                            IsInitializing = true;

                            InitUnsupportedDomains();

                            IsInitializing = false;
                        });
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

    private void TBAppendSeconds_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    // 當空白時自動設成預設的附加秒數。
                    if (string.IsNullOrEmpty(control.Text))
                    {
                        control.Text = VariableSet.DefaultAppendSeconds.ToString();
                    }

                    if (double.TryParse(control.Text, out double value))
                    {
                        if (Properties.Settings.Default.PlaylistAppendSeconds != value)
                        {
                            Properties.Settings.Default.PlaylistAppendSeconds = value;
                            Properties.Settings.Default.Save();

                            WriteLog(message: MsgSet.MsgUpdateAppendSeconds);
                        }
                    }
                    else
                    {
                        WriteLog(
                            message: MsgSet.MsgInputAValidNumber,
                            logEventLevel: LogEventLevel.Warning);

                        TBAppendSeconds.Text = Properties.Settings
                            .Default.PlaylistAppendSeconds.ToString();

                        return;
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

    private void CBEnableMpvLogVerbose_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (value)
                    {
                        if (Properties.Settings.Default.MpvNetLibLogVerbose != value)
                        {
                            Properties.Settings.Default.MpvNetLibLogVerbose = value;
                            Properties.Settings.Default.Save();

                            SetMpvPlayerLogLevel(value);
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

    private void CBEnableMpvLogVerbose_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (!value)
                    {
                        if (Properties.Settings.Default.MpvNetLibLogVerbose != value)
                        {
                            Properties.Settings.Default.MpvNetLibLogVerbose = value;
                            Properties.Settings.Default.Save();

                            SetMpvPlayerLogLevel(value);
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

    private void CBEnableOpenCCS2TWP_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (value)
                    {
                        if (Properties.Settings.Default.OpenCCS2TWP != value)
                        {
                            Properties.Settings.Default.OpenCCS2TWP = value;
                            Properties.Settings.Default.Save();
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

    private void CBEnableOpenCCS2TWP_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (!value)
                    {
                        if (Properties.Settings.Default.OpenCCS2TWP != value)
                        {
                            Properties.Settings.Default.OpenCCS2TWP = value;
                            Properties.Settings.Default.Save();
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

    private void BtnResetGridSpliiter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                RDGContentRow1.Height = new GridLength(2, GridUnitType.Star);
                RDGContentRow2.Height = new GridLength(0, GridUnitType.Auto);
                RDGContentRow3.Height = new GridLength(1, GridUnitType.Star);

                CDGContentCol1.Width = new GridLength(1, GridUnitType.Star);
                CDGContentCol2.Width = new GridLength(0, GridUnitType.Auto);
                CDGContentCol3.Width = new GridLength(1, GridUnitType.Star);

                RDTIClipPlayerRow1.Height = new GridLength(1, GridUnitType.Star);
                RDTIClipPlayerRow2.Height = new GridLength(0, GridUnitType.Auto);
                RDTIClipPlayerRow3.Height = new GridLength(1, GridUnitType.Star);

                Properties.Settings.Default.RDGContentRow1Height = RDGContentRow1.Height;
                Properties.Settings.Default.RDGContentRow3Height = RDGContentRow3.Height;
                Properties.Settings.Default.CDGContentCol1Width = CDGContentCol1.Width;
                Properties.Settings.Default.CDGContentCol3Width = CDGContentCol3.Width;
                Properties.Settings.Default.RDTIClipPlayerRow1Height = RDTIClipPlayerRow1.Height;
                Properties.Settings.Default.RDTIClipPlayerRow3Height = RDTIClipPlayerRow3.Height;
                Properties.Settings.Default.Save();
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

    #endregion

    #region yt-dlp 選項

    private void TBFormat_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    string value = control.Text;

                    if (Properties.Settings.Default.YtDlpFormat != value)
                    {
                        Properties.Settings.Default.YtDlpFormat = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();

                    // 0.5 秒後再載入 yt-dlp.conf。
                    Task.Delay(500).ContinueWith(t => LoadYtDlpConf());
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

    private void CBUseAria2_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpUseAria2 != value)
                    {
                        Properties.Settings.Default.YtDlpUseAria2 = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();

                    // 0.5 秒後再載入 yt-dlp.conf。
                    Task.Delay(500).ContinueWith(t => LoadYtDlpConf());
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

    private void CBUseAria2_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpUseAria2 != value)
                    {
                        Properties.Settings.Default.YtDlpUseAria2 = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBEmbedMetadata_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpEmbedMetadata != value)
                    {
                        Properties.Settings.Default.YtDlpEmbedMetadata = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBEmbedMetadata_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpEmbedMetadata != value)
                    {
                        Properties.Settings.Default.YtDlpEmbedMetadata = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBLiveFromStart_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpLiveFromStart != value)
                    {
                        Properties.Settings.Default.YtDlpLiveFromStart = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();

                    // 0.5 秒後再載入 yt-dlp.conf。
                    Task.Delay(500).ContinueWith(t => LoadYtDlpConf());
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

    private void CBLiveFromStart_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpLiveFromStart != value)
                    {
                        Properties.Settings.Default.YtDlpLiveFromStart = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBWaitForVideo_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpWaitForVideo != value)
                    {
                        Properties.Settings.Default.YtDlpWaitForVideo = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBWaitForVideo_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpWaitForVideo != value)
                    {
                        Properties.Settings.Default.YtDlpWaitForVideo = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBSplitChapters_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpSplitChapters != value)
                    {
                        Properties.Settings.Default.YtDlpSplitChapters = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBSplitChapters_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpSplitChapters != value)
                    {
                        Properties.Settings.Default.YtDlpSplitChapters = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBBrowserName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    ComboBox control = (ComboBox)sender;
                    ComboBoxItem selectedItem = (ComboBoxItem)control.SelectedItem;

                    string value = selectedItem.Content.ToString() ?? string.Empty;

                    if (Properties.Settings.Default.YtDlpBrowserName != value)
                    {
                        Properties.Settings.Default.YtDlpBrowserName = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void TBBrowserProfilePath_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    TextBox control = (TextBox)sender;

                    string value = control.Text;

                    if (Properties.Settings.Default.YtDlpBrowserProfilePath != value)
                    {
                        Properties.Settings.Default.YtDlpBrowserProfilePath = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void BtnSelectBrowserProfileFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    FolderBrowserDialog folderBrowserDialog = new()
                    {
                        Description = MsgSet.SelectProfilePath,
                        UseDescriptionForTitle = true,
                        SelectedPath = Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.LocalApplicationData),
                            Path.DirectorySeparatorChar.ToString())
                    };

                    DialogResult dialogResult = folderBrowserDialog.ShowDialog();

                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        TBBrowserProfilePath.Text = folderBrowserDialog.SelectedPath;
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

    private void CBCookieFromBrowser_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (Properties.Settings.Default.YtDlpCookieFromBrowser != value)
                    {
                        Properties.Settings.Default.YtDlpCookieFromBrowser = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    private void CBCookieFromBrowser_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (Properties.Settings.Default.YtDlpCookieFromBrowser != value)
                    {
                        Properties.Settings.Default.YtDlpCookieFromBrowser = value;
                        Properties.Settings.Default.Save();
                    }

                    UpdateYtDlpConf();
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

    #endregion

    #region FFmpeg 選項

    private void CBApplyFontSetting_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (value)
                    {
                        if (Properties.Settings.Default.FFmpegApplyFontSetting != value)
                        {
                            Properties.Settings.Default.FFmpegApplyFontSetting = value;
                            Properties.Settings.Default.Save();
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

    private void CBApplyFontSetting_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (!value)
                    {
                        if (Properties.Settings.Default.FFmpegApplyFontSetting != value)
                        {
                            Properties.Settings.Default.FFmpegApplyFontSetting = value;
                            Properties.Settings.Default.Save();
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

    private void CBHardwareAccelerationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    ComboBox control = (ComboBox)sender;
                    ComboBoxItem selectedItem = (ComboBoxItem)control.SelectedItem;

                    string value = selectedItem.Content.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(value))
                    {
                        foreach (VideoCardData videoCardData in CBGpuDevice.Items)
                        {
                            if (!string.IsNullOrEmpty(videoCardData.DeviceName) &&
                                videoCardData.DeviceName.Contains(value))
                            {
                                CBGpuDevice.SelectedItem = videoCardData;

                                break;
                            }
                        }

                        if (Properties.Settings.Default.FFmpegHardwareAccelerationType != value)
                        {
                            Properties.Settings.Default.FFmpegHardwareAccelerationType = value;
                            Properties.Settings.Default.Save();
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

    private void CBGpuDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    ComboBox control = (ComboBox)sender;

                    int value = control.SelectedIndex;

                    if (control.SelectedItem is VideoCardData videoCardData)
                    {
                        foreach (ComboBoxItem comboBoxItem in CBHardwareAccelerationType.Items)
                        {
                            string content = comboBoxItem.Content.ToString() ?? string.Empty;

                            if (!string.IsNullOrEmpty(videoCardData.DeviceName) &&
                                !string.IsNullOrEmpty(content) &&
                                videoCardData.DeviceName.Contains(content))
                            {
                                CBHardwareAccelerationType.SelectedItem = comboBoxItem;

                                break;
                            }
                        }
                    }

                    if (Properties.Settings.Default.FFmpegGpuDeviceIndex != value)
                    {
                        Properties.Settings.Default.FFmpegGpuDeviceIndex = value;
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

    private void CBEnableHardwareAcceleration_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? true;

                    if (value)
                    {
                        if (Properties.Settings.Default.FFmpegEnableHardwareAcceleration != value)
                        {
                            Properties.Settings.Default.FFmpegEnableHardwareAcceleration = value;
                            Properties.Settings.Default.Save();
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

    private void CBEnableHardwareAcceleration_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsInitializing)
                {
                    CheckBox control = (CheckBox)sender;

                    bool value = control.IsChecked ?? false;

                    if (!value)
                    {
                        if (Properties.Settings.Default.FFmpegEnableHardwareAcceleration != value)
                        {
                            Properties.Settings.Default.FFmpegEnableHardwareAcceleration = value;
                            Properties.Settings.Default.Save();
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

    #endregion

    #region 相依性檔案

    private void BtnForceDLDeps_Click_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnForceDLDeps,
                BtnDlYtDlp,
                BtnUpdateYtDlp,
                BtnReCreateYtDlpConf,
                BtnDLFFmpeg,
                BtnDLSubCharencParametersTxt,
                BtnDLAria2,
                BtnReCreateMpvConf,
                BtnDLLibMpv,
                BtnDLYtDlHookLua,
                BtnPlay,
                BtnPrevious,
                BtnNext,
                BtnPause,
                BtnStop,
                SVolume,
                CBNoVideo,
                CBYTQuality,
                CBPlaybackSpeed,
                BtnMute,
                MIFetchClip,
                MIDLClip,
                MIDLClipsByTheSameUrl,
                MIBatchDLClips,
                BtnBurnInSubtitle,
                BtnSplitVideo,
                MICancel
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            ShowConfirmMsgBox(MsgSet.MsgReDownloadDeps,
                new Action(() =>
                {
                    MPPlayer?.Dispose();

                    ExternalProgram.CheckDependencyFiles(
                        new Action(() =>
                        {
                            CustomFunction.RestartApplication();
                        }),
                        true);
                }),
                new Action(() =>
                {
                    CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private async void BtnDlYtDlp_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnDlYtDlp,
                BtnUpdateYtDlp,
                MIFetchClip,
                MIDLClip
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            await DownloaderUtil.DownloadYtDlp();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private void BtnUpdateYtDlp_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnDlYtDlp,
                BtnUpdateYtDlp,
                MIFetchClip,
                MIDLClip
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            ExternalProgram.UpdateYtDlp();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private void BtnReCreateYtDlpConf_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnReCreateYtDlpConf
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);
            ExternalProgram.GetOptionSet(forceCreate: true);
            CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private async void BtnDLFFmpeg_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnDLFFmpeg,
                BtnBurnInSubtitle,
                BtnSplitVideo
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            await DownloaderUtil.DownloadFFmpeg();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private async void BtnDLSubCharencParametersTxt_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnDLSubCharencParametersTxt
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            await DownloaderUtil.DownloadSubCharencParametersTxt();

            CustomFunction.BatchSetEnabled(ctrlSet, true);

            IsInitializing = true;

            InitEncodingList();

            IsInitializing = false;
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

    private async void BtnDLAria2_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnDLAria2
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            await DownloaderUtil.DownloadAria2();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
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

    private void BtnReCreateMpvConf_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnReCreateMpvConf,
                BtnDLLibMpv,
                BtnDLYtDlHookLua,
                BtnPlay,
                BtnPrevious,
                BtnNext,
                BtnPause,
                BtnStop,
                BtnMute,
                CBNoVideo
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            ExternalProgram.CreateDefaultMpvConf();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
            CustomFunction.RestartApplication();
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

    private async void BtnDLLibMpv_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnReCreateMpvConf,
                BtnDLLibMpv,
                BtnDLYtDlHookLua,
                BtnPlay,
                BtnPrevious,
                BtnNext,
                BtnPause,
                BtnStop,
                SVolume,
                CBNoVideo,
                CBYTQuality,
                CBPlaybackSpeed,
                BtnMute
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            MPPlayer?.Dispose();

            await DownloaderUtil.DownloadLibMpv();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
            CustomFunction.RestartApplication();
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

    private async void BtnDLYtDlHookLua_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Control[] ctrlSet =
            [
                BtnReCreateMpvConf,
                BtnDLLibMpv,
                BtnDLYtDlHookLua,
                BtnPlay,
                BtnPrevious,
                BtnNext,
                BtnPause,
                BtnStop,
                BtnMute,
                CBNoVideo
            ];

            CustomFunction.BatchSetEnabled(ctrlSet, false);

            await DownloaderUtil.DownloadYtDlHookLua();

            CustomFunction.BatchSetEnabled(ctrlSet, true);
            CustomFunction.RestartApplication();
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
}