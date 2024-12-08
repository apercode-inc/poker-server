using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.AdsFeature;

[JsonObject]
public class AdsRewardConfig
{
    [JsonProperty("type"), JsonConverter(typeof(StringEnumConverter))] public CurrencyType CurrencyType;
    [JsonProperty("amount")] public long Amount;
}