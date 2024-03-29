﻿using CustomToolbox.BilibiliApi.Models;
using CustomToolbox.BilibiliApi.Sets;
using CustomToolbox.Common.Extensions;
using CustomToolbox.Common.Utils;
using Downloader;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomToolbox.BilibiliApi.Functions;

/// <summary>
/// Bilibili 使用者空間 API 函式
/// </summary>
public class SpaceFunction
{
    /// <summary>
    /// 取得 tlist
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <returns>Task&lt;ReceivedObject&lt;lTList&gt;&gt;</returns>
    public static async Task<ReceivedObject<TList>> GetTList(
        HttpClient httpClient,
        string mid)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid }
            });

        // 輸出 HttpClient 的標頭資訊。
        string _ = httpClient.LogHeaders();

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        using HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
        using HttpContent content = response.Content;

        string jsonContent = await content.ReadAsStringAsync();

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(jsonContent, Options);

        return new ReceivedObject<TList>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.List?.TList
        };
    }

    /// <summary>
    /// 取得 page
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <returns>Task&lt;ReceivedObject&lt;Page&gt;&gt;</returns>
    public static async Task<ReceivedObject<Page>> GetPage(
        HttpClient httpClient,
        string mid,
        int tid)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid },
                { "tid", tid.ToString() }
            });

        // 輸出 HttpClient 的標頭資訊。
        string _ = httpClient.LogHeaders();

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        using HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
        using HttpContent content = response.Content;

        string jsonContent = await content.ReadAsStringAsync();

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(jsonContent, Options);

        return new ReceivedObject<Page>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.Page
        };
    }

    /// <summary>
    /// 取得 vlist
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <param name="tid">數值，篩選目標分區，預設值為 0</param>
    /// <param name="pn">數值，頁碼，預設值為 1 </param>
    /// <param name="ps">數值，每頁項數（最小 1 最大 50），預設值為 30</param>
    /// <returns>Task&lt;ReceivedObject&lt;List&lt;VList&gt;&gt;&gt;</returns>
    public static async Task<ReceivedObject<List<VList>>> GetVList(
        HttpClient httpClient,
        string mid,
        int tid = 0,
        int pn = 1,
        int ps = 30)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid },
                { "tid", tid.ToString() },
                { "pn", pn.ToString() },
                { "ps", ps.ToString() }
            });

        // 輸出 HttpClient 的標頭資訊。
        string _ = httpClient.LogHeaders();

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        using HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
        using HttpContent content = response.Content;

        string jsonContent = await content.ReadAsStringAsync();

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(jsonContent, Options);

        return new ReceivedObject<List<VList>>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.List?.VList
        };
    }

    /// <summary>
    /// 取得 tlist
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <returns>Task&lt;ReceivedObject&lt;lTList&gt;&gt;</returns>
    public static async Task<ReceivedObject<TList>> GetTListV2(
        HttpClient httpClient,
        string mid)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid }
            });

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        DownloadConfiguration downloadConfiguration =
            await DownloaderUtil.GetB23DownloadConfiguration(httpClient);

        DownloadService downloadService = DownloaderUtil
            .GetDownloadService(downloadConfiguration);

        Stream stream = await downloadService.DownloadFileTaskAsync(apiUrl);

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(stream, Options);

        return new ReceivedObject<TList>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.List?.TList
        };
    }

    /// <summary>
    /// 取得 page
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <returns>Task&lt;ReceivedObject&lt;Page&gt;&gt;</returns>
    public static async Task<ReceivedObject<Page>> GetPageV2(
        HttpClient httpClient,
        string mid,
        int tid)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid },
                { "tid", tid.ToString() }
            });

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        DownloadConfiguration downloadConfiguration =
            await DownloaderUtil.GetB23DownloadConfiguration(httpClient);

        DownloadService downloadService = DownloaderUtil
            .GetDownloadService(downloadConfiguration);

        Stream stream = await downloadService.DownloadFileTaskAsync(apiUrl);

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(stream, Options);

        return new ReceivedObject<Page>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.Page
        };
    }

    /// <summary>
    /// 取得 vlist
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="mid">字串，目標使用者的 mid</param>
    /// <param name="tid">數值，篩選目標分區，預設值為 0</param>
    /// <param name="pn">數值，頁碼，預設值為 1 </param>
    /// <param name="ps">數值，每頁項數（最小 1 最大 50），預設值為 30</param>
    /// <returns>Task&lt;ReceivedObject&lt;List&lt;VList&gt;&gt;&gt;</returns>
    public static async Task<ReceivedObject<List<VList>>> GetVListV2(
        HttpClient httpClient,
        string mid,
        int tid = 0,
        int pn = 1,
        int ps = 30)
    {
        string queryStringValue = await AuthFunction.GetAuthQueryString(
            httpClient,
            new Dictionary<string, string>()
            {
                { "mid", mid },
                { "tid", tid.ToString() },
                { "pn", pn.ToString() },
                { "ps", ps.ToString() }
            });

        string apiUrl = $"{UrlSet.BilibiliSpaceApiUrl}?{queryStringValue}";

        DownloadConfiguration downloadConfiguration =
            await DownloaderUtil.GetB23DownloadConfiguration(httpClient);

        DownloadService downloadService = DownloaderUtil
            .GetDownloadService(downloadConfiguration);

        Stream stream = await downloadService.DownloadFileTaskAsync(apiUrl);

        SearchRoot? searchRoot = JsonSerializer.Deserialize<SearchRoot>(stream, Options);

        return new ReceivedObject<List<VList>>()
        {
            Code = searchRoot?.Code ?? -1,
            Message = searchRoot?.Message,
            Data = searchRoot?.Data?.List?.VList
        };
    }

    /// <summary>
    /// 共用的 JsonSerializerOptions
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString |
            JsonNumberHandling.WriteAsString
    };
}