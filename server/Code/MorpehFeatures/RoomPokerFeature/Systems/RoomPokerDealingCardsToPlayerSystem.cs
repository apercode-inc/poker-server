using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDealingCardsToPlayerSystem : ISystem
{
    private const int HOLDEM_CARD_COUNT = 2;
    private const float DEALING_DELAY = 3;
    private const ulong SMALL_BLIND = 100;
    private const ulong BIG_BLIND = 200;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerCardDesk> _pokerCardDesk;
    [Injectable] private Stash<RoomPokerDealingTimer> _pokerDealingTimer;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;

    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    private Queue<CardModel> _cardModels;

    private List<RoomPokerCardNetworkModel> _networkCardsModel;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _networkCardsModel = new List<RoomPokerCardNetworkModel>();
        _cardModels = new Queue<CardModel>();
            
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<RoomPokerActive>()
            .With<RoomPokerCardDesk>()
            .With<RoomPokerDealingCardsToPlayer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var pokerCardDesk = ref _pokerCardDesk.Get(roomEntity);

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntity = playerBySeat.Value;

                _networkCardsModel.Clear();
                _cardModels.Clear();

                var cardsModel = new Queue<CardModel>();
                
                for (var i = 0; i < HOLDEM_CARD_COUNT; i++)
                {
                    if (pokerCardDesk.CardDesk.TryRandomRemove(out var cardModel))
                    {
                        cardsModel.Enqueue(cardModel);
                    }
                    else
                    {
                        throw new Exception("No cards in deck!!!");
                    }
                    
                    _networkCardsModel.Add(new RoomPokerCardNetworkModel
                    {
                        Rank = cardModel.Rank,
                        Suit = cardModel.Suit,
                    });
                }
                
                _playerCards.Set(playerEntity, new PlayerCards
                {
                    Cards = cardsModel,
                });

                var dataframe = new RoomPokerDealingCardsDataframe
                {
                    CardsForLocal = _networkCardsModel,
                };
                _server.Send(ref dataframe, playerEntity);
            }

            _roomPokerSetBlinds.Set(roomEntity, new RoomPokerSetBlinds
            {
                Small = SMALL_BLIND,
                Big = BIG_BLIND,
            });
            
            _pokerDealingTimer.Set(roomEntity,new RoomPokerDealingTimer
            {
                Timer = DEALING_DELAY,
            });
            _roomPokerDealingCardsToPlayer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
        _networkCardsModel = null;
        _cardModels = null;
    }
}