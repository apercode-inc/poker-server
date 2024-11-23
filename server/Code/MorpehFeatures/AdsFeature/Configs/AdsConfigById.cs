using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.AdsFeature.Configs;

[JsonObject]
public class AdsConfigById
{
    [JsonProperty("panel_id")] public string PanelId;
    [JsonProperty("rewarded_ads_show_cooldown")] public int RewardedAdsShowCooldown;
    [JsonProperty("ads_show_rewards")] public List<AdsRewardConfig> AdsShowRewards;
}