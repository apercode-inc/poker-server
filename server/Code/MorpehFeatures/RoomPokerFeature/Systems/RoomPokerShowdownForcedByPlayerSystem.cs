using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownForcedByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerShowdownForced> _playerShowdownForced;

    [Injectable] private NetFrameServer _server;

    private List<RoomPokerCardNetworkModel> _networkCardsModel;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _networkCardsModel = new List<RoomPokerCardNetworkModel>();
        
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerCards>()
            .With<PlayerRoomPoker>()
            .With<PlayerShowdownForced>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerCards = ref _playerCards.Get(playerEntity);
            playerCards.CardsState = CardsState.Open;

            ref var playerId = ref _playerId.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);

            _networkCardsModel.Clear();
            
            foreach (var cardModel in playerCards.Cards)
            {
                _networkCardsModel.Add(new RoomPokerCardNetworkModel
                {
                    Rank = cardModel.Rank,
                    Suit = cardModel.Suit,
                });
            }

            var dataframe = new RoomPokerSetCardsByPlayerDataframe
            {
                PlayerId = playerId.Id,
                CardsState = CardsState.Open,
                Cards = _networkCardsModel,
            };
            _server.Send(ref dataframe, playerRoomPoker.RoomEntity);

            _playerShowdownForced.Remove(playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
        _networkCardsModel = null;
    }
}