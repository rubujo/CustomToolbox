# 自定義工具箱

自定義工具箱是一款整合數個功能的整合型應用程式。

## 一、文件、相依性檔案以及網路資源

### 1. 文件

- [使用手冊](MANUAL.md)
- [更新日誌](CHANGELOG.md)

### 2. 相依性檔案

- [aria2/aria2](https://github.com/aria2/aria2)
- [libmpv](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/)
  - ※限定：`mpv-dev-x86_64-20211212-git-0e76372.7z`
  - [mpv-winbuild-cmake](https://github.com/shinchiro/mpv-winbuild-cmake)
- [sub_charenc_parameters.txt](https://trac.ffmpeg.org/attachment/ticket/2431/sub_charenc_parameters.txt)
- [yt-dlp/FFmpeg-Builds](https://github.com/yt-dlp/FFmpeg-Builds)
- [yt-dlp/yt-dlp](https://github.com/yt-dlp/yt-dlp)
- [ytdl_hook.lua](https://github.com/mpv-player/mpv/blob/master/player/lua/ytdl_hook.lua)

### 3. 網路資源

- [rubujo/CustomPlaylist](https://github.com/rubujo/CustomPlaylist)
  - [CC0 1.0 通用](https://github.com/rubujo/CustomPlaylist/blob/main/LICENSE)
- [YoutubeClipPlaylist/Playlists](https://github.com/YoutubeClipPlaylist/Playlists)
  - [MIT 授權條款](https://github.com/YoutubeClipPlaylist/Playlists/blob/master/LICENSE)

## 二、注意事項

1. 請在發布後將 `Microsoft.Playwright.dll.bak` 重新命名成 `Microsoft.Playwright.dll`。

## 三、授權資訊

因 [Xabe.FFmpeg](https://github.com/tomaszzmuda/Xabe.FFmpeg) 函式庫[授權合約](https://ffmpeg.xabe.net/license.html)的限制，此 GitHub 倉庫內，`沒有標註來源`的內容，皆採用 [CC BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) 授權條款釋出，反之皆以其來源之授權條款為準。
