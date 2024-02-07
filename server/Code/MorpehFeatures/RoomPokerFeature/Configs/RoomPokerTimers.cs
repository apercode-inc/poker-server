using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Configs;

[JsonObject]
public class RoomPokerTimers
{
    [JsonProperty("start_game_time")] public int StartGameTime;
    [JsonProperty("player_turn_time")] public int PlayerTurnTime;
}