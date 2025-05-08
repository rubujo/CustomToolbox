using OpenCCNET;
using System.Globalization;
using Whisper.net;
using Whisper.net.Ggml;
using static CustomToolbox.Common.Sets.EnumSet;

namespace CustomToolbox.Common.Utils;

/// <summary>
/// Whisper 工具
/// </summary>
public class WhisperUtil
{
    /// <summary>
    /// 取得抽樣策略類型
    /// </summary>
    /// <param name="value">字串</param>
    /// <returns>EnumSet.SamplingStrategyType</returns>
    public static SamplingStrategyType GetSamplingStrategyType(string value)
    {
        return value switch
        {
            "Default" => SamplingStrategyType.Default,
            "Greedy" => SamplingStrategyType.Greedy,
            "Beam search" => SamplingStrategyType.BeamSearch,
            _ => SamplingStrategyType.Default
        };
    }

    /// <summary>
    /// 取得模型類型
    /// </summary>
    /// <param name="value">字串</param>
    /// <returns>GgmlType</returns>
    public static GgmlType GetModelType(string value)
    {
        return value switch
        {
            "Tiny" => GgmlType.Tiny,
            "Tiny En" => GgmlType.TinyEn,
            "Base" => GgmlType.Base,
            "Base En" => GgmlType.BaseEn,
            "Small" => GgmlType.Small,
            "Small En" => GgmlType.SmallEn,
            "Medium" => GgmlType.Medium,
            "Medium En" => GgmlType.MediumEn,
            "Large V1" => GgmlType.LargeV1,
            "Large V2" => GgmlType.LargeV2,
            "Large V3" => GgmlType.LargeV3,
            "Large V3 Turbo" => GgmlType.LargeV3Turbo,
            _ => GgmlType.Base
        };
    }

    /// <summary>
    /// 取得量化類型
    /// </summary>
    /// <param name="value">字串</param>
    /// <returns>QuantizationType</returns>
    public static QuantizationType GetQuantizationType(string value)
    {
        return value switch
        {
            "No Quantization" => QuantizationType.NoQuantization,
            "Q4_0" => QuantizationType.Q4_0,
            "Q4_1" => QuantizationType.Q4_1,
            "Q5_0" => QuantizationType.Q5_0,
            "Q5_1" => QuantizationType.Q5_1,
            "Q8_0" => QuantizationType.Q8_0,
            _ => QuantizationType.NoQuantization
        };
    }

    /// <summary>
    /// 取得模型檔案的名稱
    /// </summary>
    /// <param name="ggmlType">GgmlType</param>
    /// <param name="quantizationType">QuantizationType，預設值為 QuantizationType.NoQuantization</param>
    /// <returns>字串</returns>
    public static string GetModelFileName(
        GgmlType ggmlType,
        QuantizationType quantizationType = QuantizationType.NoQuantization)
    {
        string mainFileName = ggmlType switch
        {
            GgmlType.Tiny => "ggml-tiny",
            GgmlType.TinyEn => "ggml-tiny.en",
            GgmlType.Base => "ggml-base",
            GgmlType.BaseEn => "ggml-base.en",
            GgmlType.Small => "ggml-small",
            GgmlType.SmallEn => "ggml-small.en",
            GgmlType.Medium => "ggml-medium",
            GgmlType.MediumEn => "ggml-medium.en",
            GgmlType.LargeV1 => "ggml-large-v1",
            GgmlType.LargeV2 => "ggml-large-v2",
            GgmlType.LargeV3 => "ggml-large-v3",
            GgmlType.LargeV3Turbo => "ggml-large-v3-turbo",
            _ => string.Empty
        },
        subFileName = quantizationType switch
        {
            QuantizationType.NoQuantization => string.Empty,
            QuantizationType.Q4_0 => "q4_0",
            QuantizationType.Q4_1 => "q4_1",
            QuantizationType.Q5_0 => "q5_0",
            QuantizationType.Q5_1 => "q5_1",
            QuantizationType.Q8_0 => "q8_0",
            _ => string.Empty
        },
        extName = ".bin";

        if (string.IsNullOrEmpty(mainFileName))
        {
            return string.Empty;
        }
        else
        {
            if (!string.IsNullOrEmpty(subFileName))
            {
                subFileName = $".{subFileName}";
            }

            return $"{mainFileName}{subFileName}{extName}";
        }
    }

    /// <summary>
    /// 列印時間（點）
    /// </summary>
    /// <param name="timeSpan">TimeSpan</param>
    /// <returns>字串</returns>
    public static string PrintTime(TimeSpan timeSpan) =>
        timeSpan.ToString("hh':'mm':'ss'.'fff", CultureInfo.InvariantCulture);

    /// <summary>
    /// 列印時間（逗號）
    /// </summary>
    /// <param name="timeSpan">TimeSpan</param>
    /// <returns>字串</returns>
    public static string PrintTimeWithComma(TimeSpan timeSpan) =>
        timeSpan.ToString("hh':'mm':'ss','fff", CultureInfo.InvariantCulture);

    /// <summary>
    /// 取得 WhisperProcessor
    /// </summary>
    /// <param name="whisperProcessorBuilder">WhisperProcessorBuilder</param>
    /// <param name="samplingStrategyType">SamplingStrategyType，預設值為 SamplingStrategyType.Default</param>
    /// <param name="beamSize">beamSize，用於 SamplingStrategyType.BeamSearch，預設值為 5</param>
    /// <param name="patience">patience，用於 SamplingStrategyType.BeamSearch，預設值為 -0.1f</param>
    /// <param name="bestOf">bestOf，用於 SamplingStrategyType.Greedy，預設值為 1</param>
    /// <returns>WhisperProcessor</returns>
    public static WhisperProcessor GetWhisperProcessor(
        WhisperProcessorBuilder whisperProcessorBuilder,
        SamplingStrategyType samplingStrategyType = SamplingStrategyType.Default,
        int beamSize = 5,
        float patience = -0.1f,
        int bestOf = 1)
    {
        WhisperProcessor whisperProcessor;

        switch (samplingStrategyType)
        {
            default:
            case SamplingStrategyType.Default:
                whisperProcessor = whisperProcessorBuilder.Build();

                break;
            case SamplingStrategyType.Greedy:
                GreedySamplingStrategyBuilder greedySamplingStrategyBuilder =
                    (GreedySamplingStrategyBuilder)whisperProcessorBuilder
                        .WithGreedySamplingStrategy();

                greedySamplingStrategyBuilder.WithBestOf(bestOf);

                whisperProcessor = greedySamplingStrategyBuilder
                    .ParentBuilder.Build();

                break;
            case SamplingStrategyType.BeamSearch:
                BeamSearchSamplingStrategyBuilder beamSearchSamplingStrategyBuilder =
                    (BeamSearchSamplingStrategyBuilder)whisperProcessorBuilder
                        .WithBeamSearchSamplingStrategy();

                beamSearchSamplingStrategyBuilder
                    .WithBeamSize(beamSize)
                    .WithPatience(patience);

                whisperProcessor = beamSearchSamplingStrategyBuilder
                    .ParentBuilder.Build();

                break;
        }

        return whisperProcessor;
    }

    /// <summary>
    /// 取得 SegmentData 的文字
    /// </summary>
    /// <param name="segmentData">SegmentData</param>
    /// <returns>字串，文字內容</returns>
    public static string GetSegmentDataText(SegmentData segmentData)
    {
        if (Properties.Settings.Default.OpenCCS2TWP)
        {
            return ZhConverter.HansToTW(segmentData.Text, true).TrimStart();
        }
        else
        {
            return segmentData.Text.TrimStart();
        }
    }
}