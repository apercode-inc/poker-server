using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudShowCardsSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerShowOrHideTimer> _playerShowOrHideTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;

    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }
    

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudShowCardsRequestDataframe>(Handler);
    }
    
    private void Handler(RoomPokerHudShowCardsRequestDataframe dataframe, int clientId)
    {
        if (!_playerStorage.TryGetPlayerById(clientId, out var playerEntity))
        {
            return;
        }

        ref var playerShowOrHideTimer = ref _playerShowOrHideTimer.Get(playerEntity, out var showOrHideTimerExist);

        if (!showOrHideTimerExist)
        {
            return;
        }
        
        _playerShowOrHideTimer.Remove(playerEntity);

        ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity, out var playerRoomPokerExist);

        if (!playerRoomPokerExist || playerRoomPoker.RoomEntity.IsNullOrDisposed())
        {
            return;
        }

        var roomEntity = playerRoomPoker.RoomEntity;

        ref var playerCards = ref _playerCards.Get(playerEntity);
        ref var playerId = ref _playerId.Get(playerEntity);
        
        playerCards.CardsState = CardsState.Open;
        var cards = playerCards.Cards;
        
        var cardsNetworkModels = new List<RoomPokerCardNetworkModel>();
        
        //todo может вынести в метод, в RoomPokerShowdownSystem похожая логика
        foreach (var card in cards)
        {
            cardsNetworkModels.Add(new RoomPokerCardNetworkModel
            {
                Rank = card.Rank,
                Suit = card.Suit,
            });
        }
        
        var showdownModels = new List<RoomPokerShowdownNetworkModel>
        {
            new()
            {
                PlayerId = playerId.Id,
                Cards = cardsNetworkModels,
            }
        };

        var dataframeShowdown = new RoomPokerShowdownDataframe
        {
            IsBankSync = false,
            ShowdownModels = showdownModels,
        };
        _server.SendInRoom(ref dataframeShowdown, roomEntity);
        
        var resetTimerDataframe = new RoomPokerResetTurnTimerDataframe
        {
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref resetTimerDataframe, roomEntity);
        
        _roomPokerShowOrHideCardsActivate.Set(playerRoomPoker.RoomEntity);
        
        playerShowOrHideTimer.TimeCurrent = playerShowOrHideTimer.TimeMax;
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudShowCardsRequestDataframe>(Handler);
    }
}