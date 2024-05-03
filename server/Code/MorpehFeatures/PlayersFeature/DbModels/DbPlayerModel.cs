namespace server.Code.MorpehFeatures.PlayersFeature.DbModels;

public class DbPlayerModel
{
    public string unique_id { get; set; }
    public string nickname { get; set; }
    public int level { get; set; }
    public int experience { get; set; }
    public long chips { get; set; }
    public long gold { get; set; }
    public long stars { get; set; }
    public int avatar_id { get; set; }
    public string avart_url { get; set; }
    public DateTime registration_date { get; set; }
}