using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
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
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;
    
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;
    [Injectable] private NetFrameServer _server;
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
            _roomPokerCardDeskService.ReturnCardsInDeskToTable(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;
                
                _playerTurnTimerReset.Set(player);
                _roomPokerCardDeskService.ReturnCardsInDeskToPlayer(roomEntity, player);

                ref var playerId = ref _playerId.Get(player);

                var cardsDataframe = new RoomPokerSetCardsByPlayerDataframe
                {
                    CardsState = CardsState.Empty,
                    PlayerId = playerId.Id,
                };
                _server.SendInRoom(ref cardsDataframe, roomEntity);

                ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
                playerPokerCurrentBet.Value = 0;
            }

            ref var roomPokerPlayersGivenBank = ref _roomPokerPlayersGivenBank.Get(roomEntity);
            
            //todo без учета all-in и все такое
            var winnings = roomPokerBank.OnTable / roomPokerPlayersGivenBank.Players.length;
            
            foreach (var player in roomPokerPlayersGivenBank.Players)
            {
                _currencyPlayerService.TryGiveBank(roomEntity, player, winnings);
            }

            roomPokerBank.Total = 0;
            roomPokerBank.OnTable = 0;
            
            var cardsToTableDataframe = new RoomPokerSetCardsToTableDataframe
            {
                Bank = roomPokerBank.OnTable,
                CardToTableState = CardToTableState.PreFlop,
                Cards = new List<RoomPokerCardNetworkModel>(),
            };
            _server.SendInRoom(ref cardsToTableDataframe, roomEntity);
            
            if (_roomPokerActive.Has(roomEntity))
            {
                var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
                
                _roomPokerNextDealingTimer.Set(roomEntity,new RoomPokerNextDealingTimer
                {
                    Value = config.DelayBeforeNextDealingCards
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