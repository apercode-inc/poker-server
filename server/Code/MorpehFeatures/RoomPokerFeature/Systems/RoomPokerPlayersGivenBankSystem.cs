using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayersGivenBankSystem : ISystem
{
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;
    [Injectable] private Stash<RoomPokerSetCardsTickTimer> _roomPokerSetCardsTickTimer;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerShowdownTimer> _roomPokerShowdownTimer;

    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    [Injectable] private ConfigsService _configsService;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerMaxBet>()
            .With<RoomPokerStats>()
            .With<RoomPokerBank>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerPlayersGivenBank>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

            roomPokerMaxBet.Value = roomPokerStats.BigBet / 2;
            
            ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);
            roomPokerBank.OnTable = roomPokerBank.Total;
            
            _roomPokerSetCardsTickTimer.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerPlayersGivenBank = ref _roomPokerPlayersGivenBank.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;
                
                _playerTurnTimerReset.Set(player);

                ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
                playerPokerCurrentBet.Value = 0;
            }

            //todo без учета all-in и все такое
            var winnings = roomPokerBank.OnTable / roomPokerPlayersGivenBank.Players.length;
            
            foreach (var player in roomPokerPlayersGivenBank.Players)
            {
                _currencyPlayerService.TryGiveBank(roomEntity, player, winnings);
            }

            roomPokerBank.Total = 0;
            roomPokerBank.OnTable = 0;

            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);

            if (_roomPokerActive.Has(roomEntity))
            {
                _roomPokerShowdownTimer.Set(roomEntity, new RoomPokerShowdownTimer
                {
                    Value = config.DelayShowdown,
                });
            }
            else
            {
                _roomPokerCleanupTimer.Set(roomEntity, new RoomPokerCleanupTimer
                {
                    Value = 0,
                });
            }

            _roomPokerPlayersGivenBank.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}