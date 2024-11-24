using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCleanupGameSystem : ISystem
{
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;

    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;
    [Injectable] private Stash<PlayerAllin> _playerAllin;
    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;
    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCleanupTimer>()
            .With<RoomPokerPlayers>()
            .Without<RoomPokerShowOrHideCards>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerNextDealingTimer = ref _roomPokerCleanupTimer.Get(roomEntity);
            
            if (!_roomPokerActive.Has(roomEntity))
            {
                roomPokerNextDealingTimer.Value = 0;
            }

            roomPokerNextDealingTimer.Value -= deltaTime;

            if (roomPokerNextDealingTimer.Value > 0)
            {
                continue;
            }

            CleanupPlayers(roomEntity);
            CleanupGame(roomEntity);
        }
    }

    private void CleanupPlayers(Entity roomEntity)
    {
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        roomPokerPlayers.PlayerPotModels.Clear();

        foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
        {
            var player = markedPlayer.Value;

            _roomPokerCardDeskService.ReturnCardsInDeskToPlayer(roomEntity, player);

            ref var playerId = ref _playerId.Get(player);

            _playerTurnCompleteFlag.Remove(player);
            _playerPokerCombination.Remove(player);
            _playerAllin.Remove(player);

            ref var playerCards = ref _playerCards.Get(player);
            playerCards.CardsState = CardsState.Empty;
            playerCards.Cards = null;

            var cardsDataframe = new RoomPokerSetCardsByPlayerDataframe
            {
                CardsState = CardsState.Empty,
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref cardsDataframe, roomEntity);
        }
    }
    
    private void CleanupGame(Entity roomEntity)
    {
        var cardsToTableDataframe = new RoomPokerSetCardsToTableDataframe
        {
            Bank = 0,
            CardToTableState = CardToTableState.PreFlop,
            Cards = new List<RoomPokerCardNetworkModel>(),
        };
        _server.SendInRoom(ref cardsToTableDataframe, roomEntity);

        var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);

        _roomPokerCleanupTimer.Remove(roomEntity);
        _roomPokerCardDeskService.ReturnCardsInDeskToTable(roomEntity);
        _roomPokerOnePlayerRoundGame.Remove(roomEntity);

        if (!_roomPokerActive.Has(roomEntity))
        {
            var dataframe = new RoomPokerStopGameResetTimerDataframe();
            _server.SendInRoom(ref dataframe, roomEntity);
            return;
        }

        _roomPokerNextDealingTimer.Set(roomEntity, new RoomPokerNextDealingTimer
        {
            Value = config.DelayBeforeNextDealingCards,
        });
    }

    public void Dispose()
    {
        _filter = null;
    }
}