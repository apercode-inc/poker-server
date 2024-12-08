using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.AdsFeature.Configs;

[JsonObject]
public class AdsConfig
{
    [JsonProperty("rewards_for_panels")] public Dictionary<string, AdsConfigById> RewardsForPanels;
}