using Newtonsoft.Json;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.AdsFeature;

[JsonObject]
public class AdsRewardConfig
{
    [JsonIgnore]
    public CurrencyType CurrencyType
    {
        get => (CurrencyType)CurrencyTypeId;
        set => CurrencyTypeId = (int)value;
    }
    
    [JsonProperty("type")] public int CurrencyTypeId;
    [JsonProperty("amount")] public long Amount;
}