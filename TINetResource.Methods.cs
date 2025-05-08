using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using CustomToolbox.Common.Utils;
using Serilog.Events;

namespace CustomToolbox;

/// <summary>
/// TINetResource 的方法
/// </summary>
public partial class WMain
{
    /// <summary>
    /// 初始化網路資源
    /// </summary>
    private void InitNetResurce()
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                CBNetPlaylists.ItemsSource = null;

                List<ClipListData> dataSource1 = await ClipListUtil.GetNetPlaylists();

                CBNetPlaylists.ItemsSource = dataSource1;
                CBNetPlaylists.DisplayMemberPath = nameof(ClipListData.Name);
                CBNetPlaylists.SelectedValuePath = nameof(ClipListData.Path);

                if (dataSource1.Count > 0)
                {
                    CBNetPlaylists.SelectedIndex = 0;
                }

                CBLocalClipLists.ItemsSource = null;

                List<ClipListData> dataSource2 = ClipListUtil.GetLocalClipLists();

                CBLocalClipLists.ItemsSource = dataSource2;
                CBLocalClipLists.DisplayMemberPath = nameof(ClipListData.Name);
                CBLocalClipLists.SelectedValuePath = nameof(ClipListData.Path);

                if (dataSource2.Count > 0)
                {
                    CBLocalClipLists.SelectedIndex = 0;
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
    /// 初始化排除字詞
    /// </summary>
    private void InitB23ClipListExcludedPhrases()
    {
        try
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                char[] separators = [';'];

                string[] tempValue = Properties.Settings.Default
                    .B23ClipListExcludedPhrases.Split(
                        separators,
                        StringSplitOptions.RemoveEmptyEntries);

                string value = string.Join(Environment.NewLine, tempValue);

                TBB23ClipListExcludedPhrases.Text = value;
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