namespace CustomToolbox.Common.Models;

/// <summary>
/// 短片清單資料
/// </summary>
public class ClipListData(string name, string path)
{
    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 路徑
    /// </summary>
    public string Path { get; set; } = path;

    /// <summary>
    /// 轉換成字串
    /// </summary>
    /// <returns>字串</returns>
    public override string ToString()
    {
        return Name;
    }
}