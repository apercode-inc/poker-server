using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.AdsFeature.Configs;

[JsonObject]
public class AdsConfig
{
    [JsonProperty("rewarded_ads_show_cooldown")] public int RewardedAdsShowCooldown;
    [JsonProperty("rewards_for_panels")] public List<AdsConfigById> RewardsForPanels;
}