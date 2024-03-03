# 自定義工具箱 - 更新日誌

- v1.2.3
  1. 更新第三方函式庫。
  2. 更新預設的使用者代理字串。（Google Chrome v122）
  3. 配合功能異動更新 MANUAL.md。
  4. 調整快速鍵，將原先 T 鍵的功能移至 Y 鍵，將 T 鍵的功能變更成開／關彈出視窗顯示在最上層。
- v1.2.2
  1. 更新第三方函式庫。
  2. 更新預設的使用者代理字串。（Google Chrome v120）
  3. 修正 Whisper 功能的作業進度回報使用者控制項顯示異常的問題。
  4. 配合 B 站新的風險控制校驗機制進行調整。
  5. 重新調整部分使用者控制項的鎖定機制。
  6. 調整 yt-dlp 的訊息輸出機制。
- v1.2.1
  1. 更新第三方函式庫。
  2. 配合 Whisper.net 函式庫的異動調整程式碼。
  3. 更新 yt-dlp 已更新的判斷字詞。
  4. 更新 Bilibili API TList.cs 的異動。
  5. Whisper 相關的方法擴充提示詞的參數。（未開放使用者使用）
  6. 加入 yt-dlp 執行 --update-to 的方法。（未開放使用者使用）
- v1.2.0
  1. 遷移到 .NET 8。
  2. 更新第三方函式庫。
  3. 更新預設的使用者代理字串。 （Google Chrome v119）
  4. 更新 Aria2 的下載網址。(v1.37.0) 
  5. 調整 DataGrid 控制項的樣式。 
- v1.1.1
  1. 修正 RTBLog 不會自動捲到底的問題。 
- v1.1.0
  1. 移除 ModernWpf 函式庫。
  2. 加入 Whisper 功能。（CPU 版）
  3. 更新第三方函式庫。
  4. 配合程式的異動，更新語言檔案。
  5. 配合 1. 將 UI 排版重新調整。
  6. 加入從網路播放清單複製 MID 的功能。（僅限 Bilibili 的播放清單） 
  7. 更新 README.md。
  8. 調整 Log 的輸出方式，改為使用 Serilog 函式庫來輸出 Log。
  9. 將 TBLog 改為 RTBLog，並改用 RichTextBox 重新實現功能。
  10. 調整開啟資料夾按鈕位置，並新增開啟 Temp、Models 等資料夾的按鈕。
  12. 加入與使用者代理字串的新欄位，並提供自行設定的功能。
  13. 加入新的 Bilibili 風險控制機制的實做。
- v1.0.15
  1. 更新 README.md。
  2. 更新預設的使用者代理字串。（Google Chrome v115）
  3. 將判斷字數限制的部分改為使用 StringInfo 來處理。
  4. 加入 B 站 Wbi 驗證機制。
  5. 更新 Downloader 函式庫至 3.0.6 版。
  6. 更新 HtmlAgilityPack 函式庫至 1.11.50 版。
  7. 更新 Microsoft.Playwright 函式庫至 1.36.0 版。
  8. 更新 YoutubeDLSharp 函式庫至 1.0.0 版。
  9. 更新 DiscordRichPresence 函式庫至 1.4.20 版。
  10. 加入 Whisper 相關函式庫。
  11. 加入新的預設資料夾。
  12. 加入 FFmpeg 轉換成 WAV 檔案的相關程式碼。
  13. 調高 Playwright 的預設等待秒數，從 1 秒調高成 2 秒。
- v1.0.14
  1. 更新 Microsoft.Playwright 函式庫至 1.34.0 版。
  2. 修正 MANUAL.md 內錯誤的內容。
  3. 移除不在需要的 TODO 註解。
  4. 調整載入上一版本的設定值的相關程式碼。
- v1.0.13
  1. 修正短片列表時間欄位不可以直接輸入時間格式字串的問題。
- v1.0.12
  1. 更新預設的使用者代理字串。（Google Chrome v113）
  2. 更新 Microsoft.Playwright 函式庫至1.33.0 版。
- v1.0.11
  1. 更新預設的使用者代理字串。（Google Chrome v112）
  2. 變更查詢使用者代理字串的網址，改為使用 WhatIsMyBrowser.com 網站。
  3. 更新 H.NotifyIcon.Wpf 函式庫至 2.0.108 版。
  4. 更新 Microsoft.Playwright 函式庫至1.32.0 版。
  5. 更新 YoutubeDLSharp 函式庫至 1.0.0-beta6 版。
- v1.0.10
  1. 更新 Downloader 函式庫至 3.0.4 版。
  2. 更新 H.NotifyIcon.Wpf 函式庫至 2.0.99 版。
  3. 更新 Xabe.FFmpeg 函式庫至 5.2.6 版。
  4. 更新預設的使用者代理字串。
- v1.0.9
  1. 更新 H.NotifyIcon.Wpf 函式庫至 2.0.93 版。
  2. 更新 Microsoft.Playwright 函式庫至 1.31.1 版。
  3. 更新 YoutubeDLSharp 函式庫至 1.0.0-beta4 版。
  4. 更新 MANUAL.md。
  5. 更新語系檔案。
  6. 對 " Bilibili 短片清單產生器" 功能，加入"檢查網址"選項。
  7. 更新 BilibiliApi\README.md 的內容。
  8. 更新 BiliBili API 的網址。
  9. 改寫 yt-dlp 的下載方式。
  10. 調整 *.csproj 的設定，在發布時輸出 CHANGELOG.md 以及 MANUAL.md 等檔案。
- v1.0.8
  1. 針對 DataGrid 的排序機制，調整播放時的選取播放項目的策略。
  2. 更新 Microsoft.Playwright 函式庫至 1.31.0 版。
  3. 調整 Client Hints 相關設定方式。
  4. 加入更新應用程式設定值的功能。
  5. 針對隨機重新排序加入依照 DataGrid 的排序機制排序的功能。
- v1.0.7
  1. 修改 mid 的類型，將類型從 int 改為 long，因為目前的 mid 已經會超過 int 的範圍，從而造成 JSON 資料解析失敗。
  2. 更新 Xabe.FFmpeg 至 5.2.5 版。
- v1.0.6
  1. 更新語系檔案，並修正尚未 l10n 化的內容。
  2. 修正在應用程式切換主題時，系統工具列圖示的右鍵選單的主題，不會同步更新的問題。
  3. 將部分 Button 控制項，改為使用 AppBarButton 控制項。
  4. 針對部分按鈕以及選單選項，加入圖示。
  5. 加入時間標記編輯器相關功能。
  6. 更新預設的使用者代理字串。 (Google Chrome)
  7. 更新預設的 Sec-CH-UA 相關標頭資料。 (Google Chrome)
- v1.0.5
  1. 更新函式庫 H.NotifyIcon.Wpf 至 2.0.86 版。
  2. 重新調整 SSeek 控制項的事件 SSeek_KeyUpEvent() 的內部程式判定邏輯。
  3. 修正部分影片透過拖曳時間軸至結束時間點會造成應用程式崩潰的問題。
  4. 更新語系檔案。
- v1.0.4
  1. 修正無法使用方向鍵控制時間軸的問題。
  2. 針對 YouTube 的網址加入判斷，讓 YouTube 的網址可以吃 YouTube 畫質設定。
  3. 修正 libmpv 在 yt-dlp 發生錯誤後接下來的播放會沒有反應的問題。
  4. 更新第三方函示庫 Downloader 至 v3.0.3 版。
  5. 相關文件更新。
- v1.0.3
  - 修正匯出時間標記文字檔案時網址資訊會出錯的問題。
- v1.0.2
  1. 加入批次下載短片功能。
  2. 調整短片工具頁籤內容的排版，針對 FFmpeg 相關功能的部分加入 GroupBox 控制項以區分。
  3. 更新第三方函式庫 Xabe.FFmpeg 至 v5.2.4 版。
  4. 更新第三方函式庫 Microsoft.Playwright 至 v1.30.0 版。
  5. 針對 yt-dlp 的下載結果補上對應的輸出訊息。
  6. 更新語系檔案。
  7. 更新 README.md。
- v1.0.1
  - 修正呼叫 SetRichPresence() 方法會發生例外的問題。
- v1.0.0
  - 初始發布。