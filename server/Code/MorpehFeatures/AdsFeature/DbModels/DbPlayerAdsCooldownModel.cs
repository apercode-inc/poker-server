namespace server.Code.MorpehFeatures.AdsFeature.DbModels;

public class DbPlayerAdsCooldownModel
{
    public string player_id { get; set; }
    public string panel_id { get; set; }
    public int end_timestamp { get; set; }
}