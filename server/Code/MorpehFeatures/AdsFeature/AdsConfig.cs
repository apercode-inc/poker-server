using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.AdsFeature;

[JsonObject]
public class AdsConfig
{
    [JsonProperty("rewarded_ads_show_cooldown")] public int RewardedAdsShowCooldown;
    [JsonProperty("rewarded_ads_show_cooldown_on_start")] public int RewardedAdsShowCooldownOnStart;
    [JsonProperty("ads_show_rewards")] public List<AdsRewardConfig> AdsShowRewards;
}