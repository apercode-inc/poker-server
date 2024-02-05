using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.PokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.PokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerDealingCardsToPlayerSystem : ISystem
{
    private const int HOLDEM_CARD_COUNT = 2;
    private const float DEALING_DELAY = 3;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PokerDealingCardsToPlayer> _pokerDealingCardsToPlayer;
    [Injectable] private Stash<PokerCardDesk> _pokerCardDesk;
    [Injectable] private Stash<PokerDealingTimer> _pokerDealingTimer;

    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    private Queue<CardModel> _cardModels;

    private List<PokerCardNetworkModel> _networkCardsModel;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _networkCardsModel = new List<PokerCardNetworkModel>();
        _cardModels = new Queue<CardModel>();
            
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<PokerActive>()
            .With<PokerCardDesk>()
            .With<PokerDealingCardsToPlayer>()
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
                    
                    _networkCardsModel.Add(new PokerCardNetworkModel
                    {
                        Rank = cardModel.Rank,
                        Suit = cardModel.Suit,
                    });
                }
                
                _playerCards.Set(playerEntity, new PlayerCards
                {
                    Cards = cardsModel,
                });

                var dataframe = new PokerDealingCardsDataframe
                {
                    Cards = _networkCardsModel,
                };
                _server.Send(ref dataframe, playerEntity);
            }
            
            //todo тут надо ещё блайнды проставить лучше навестить компонент и в отдельной системе
            
            _pokerDealingTimer.Set(roomEntity,new PokerDealingTimer
            {
                Timer = DEALING_DELAY,
            });
            _pokerDealingCardsToPlayer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
        _networkCardsModel = null;
        _cardModels = null;
    }
}