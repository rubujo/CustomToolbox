using CustomToolbox.Common.Models;
using CustomToolbox.Common.Sets;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CustomToolbox.Common.Extensions;

/// <summary>
/// List 的擴充方法
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// 取得短片列表
    /// </summary>
    /// <param name="listClipData">List&lt;ClipData&gt;</param>
    /// <param name="filePath">字串，檔案儲存的路徑</param>
    /// <param name="exportJsonc">布林值，是否匯出 *.jsonc 格式，預設值為 false</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task SaveFile(
        this List<ClipData> listClipData,
        string filePath,
        bool exportJsonc = false,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        List<List<object>> listObject = [];

        foreach (ClipData clipData in listClipData)
        {
            listObject.Add(
            [
                clipData.VideoUrlOrID ?? string.Empty,
                clipData.StartTime.TotalSeconds,
                clipData.EndTime.TotalSeconds,
                clipData.Name ?? string.Empty,
                clipData.SubtitleFileUrl ?? string.Empty
            ]);
        }

        object dataSource = exportJsonc ? listObject : listClipData;

        using FileStream fileStream = new(
            filePath,
            new FileStreamOptions()
            {
                Access = FileAccess.ReadWrite,
                Mode = FileMode.Create,
                Share = FileShare.ReadWrite
            });

        await JsonSerializer.SerializeAsync(
            fileStream,
            dataSource,
            VariableSet.SharedJSOptions,
            ct);

        // 編輯已儲存的 JSONC 檔案，加入檔案標頭內容。
        StringBuilder stringBuilder = new();

        using StreamReader streamReader = new(fileStream);

        // 判斷是否要加入檔案標頭。
        if (exportJsonc)
        {
            string strJSONCHeader = $"/**{Environment.NewLine}" +
                $" * 歌單格式為JSON with Comments{Environment.NewLine}" +
                $" * [\"VideoID\", StartTime, EndTime, \"Title\", \"SubSrc\"]{Environment.NewLine}" +
                $" * VideoID: 必須用引號包住，為字串型態。{Environment.NewLine}" +
                $" * StartTime: 只能是非負數。如果要從頭播放，輸入0{Environment.NewLine}" +
                $" * EndTime: 只能是非負數。如果要播放至尾，輸入0{Environment.NewLine}" +
                $" * Title?: 必須用引號包住，為字串型態{Environment.NewLine}" +
                $" * SubSrc?: 必須用雙引號包住，為字串型態，可選{Environment.NewLine}" +
                $" */{Environment.NewLine}";

            stringBuilder.Append(strJSONCHeader);
        }

        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

        while (!streamReader.EndOfStream)
        {
            stringBuilder.AppendLine(streamReader.ReadLine());
        }

        await fileStream.DisposeAsync();

        using StreamWriter streamWriter = new(
            path: filePath,
            append: false,
            encoding: Encoding.UTF8);

        // 移除最後的新行。
        string jsonContent = stringBuilder.ToString().TrimEnd('\r', '\n');

        streamWriter.Write(jsonContent);

        await streamWriter.DisposeAsync();
    }
}