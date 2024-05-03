using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.PlayersFeature.Configs;

[JsonObject]
public class PlayerCreateConfig
{
    [JsonProperty("nickname_length")] public int NicknameLength;
    [JsonProperty("start_chips")] public int StartChips;
    [JsonProperty("start_gold")] public int StartGold;
    [JsonProperty("start_stars")] public int StartStars;
}