using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Configs;

[JsonObject]
public class RoomPokerSettingsConfig
{
    [JsonProperty("start_game_time")] public int StartGameTime;
    [JsonProperty("player_turn_time")] public int PlayerTurnTime;
    [JsonProperty("dealing_cards_time")] public int DealingCardsTime;
    [JsonProperty("player_turn_time_fast")] public int PlayerTurnTimeFast;
    [JsonProperty("seat_counts")] public List<int> SeatCounts;
    [JsonProperty("bets")] public List<RoomPokerBetsConfig> Bets;
}

[JsonObject]
public class RoomPokerBetsConfig
{
    [JsonProperty("blind_big")] public long BlindBig;
    [JsonProperty("contribution")] public long Contribution;
}