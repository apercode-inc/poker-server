using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Configs;

[JsonObject]
public class RoomPokerSettingsConfig
{
    [JsonProperty("start_game_time")] public int StartGameTime;
    [JsonProperty("player_turn_time")] public int PlayerTurnTime;
    [JsonProperty("player_turn_fast_time")] public int PlayerTurnTimeFast;
    [JsonProperty("dealing_cards_time")] public float DealingCardsTime;
    [JsonProperty("dealing_card_time_to_table")] public int DealingCardTimeToTable;
    [JsonProperty("delay_pay_out")] public int DelayPayOut;
    [JsonProperty("delay_showdown")] public int DelayShowdown;
    [JsonProperty("delay_cleanup")] public int DelayCleanup;
    [JsonProperty("delay_before_next_dealing_cards")] public int DelayBeforeNextDealingCards;
    [JsonProperty("seat_counts")] public List<int> SeatCounts;
    [JsonProperty("bets")] public List<RoomPokerBetsConfig> Bets;
}

[JsonObject]
public class RoomPokerBetsConfig
{
    [JsonProperty("blind_big")] public long BlindBig;
    [JsonProperty("contribution")] public long Contribution;
}