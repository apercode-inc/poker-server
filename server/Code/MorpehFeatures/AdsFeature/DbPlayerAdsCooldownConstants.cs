namespace server.Code.MorpehFeatures.AdsFeature;

public static class DbPlayerAdsCooldownConstants
{
    public const float WriteCooldownInDbTimerThreshold = 3f;
    
    public const string TableName = "players_ads_cooldowns";
    public const string PlayerId = "player_id";
    public const string PanelId = "panel_id";
    public const string EndTimestamp = "end_timestamp";
}