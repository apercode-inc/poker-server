using Newtonsoft.Json;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Configs;

[JsonObject]
public class RoomPokerSettingsConfig
{
    [JsonProperty("start_game_time")] public float StartGameTime;
    [JsonProperty("player_turn_time")] public float PlayerTurnTime;
    [JsonProperty("player_turn_fast_time")] public float PlayerTurnTimeFast;
    [JsonProperty("player_turn_showdown_time")] public float PlayerTurnShowdownTime;
    [JsonProperty("player_turn_showdown_fast_time")] public float PlayerTurnShowdownTimeFast;
    [JsonProperty("dealing_cards_time")] public float DealingCardsTime;
    [JsonProperty("dealing_card_time_to_table")] public float DealingCardTimeToTable;
    [JsonProperty("delay_pay_out")] public float DelayPayOut;
    [JsonProperty("delay_showdown")] public float DelayShowdown;
    [JsonProperty("delay_cleanup")] public float DelayCleanup;
    [JsonProperty("delay_before_next_dealing_cards")] public float DelayBeforeNextDealingCards;
    [JsonProperty("seat_counts")] public List<int> SeatCounts;
    [JsonProperty("bets")] public List<RoomPokerBetsConfig> Bets;
}

[JsonObject]
public class RoomPokerBetsConfig
{
    [JsonProperty("blind_big")] public long BlindBig;
    [JsonProperty("contribution")] public long Contribution;
    [JsonProperty("min_contribution")] public long MinContribution;
}