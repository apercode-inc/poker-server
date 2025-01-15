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
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDealingCardsToPlayerSystem : ISystem
{
    private const int HOLDEM_CARD_COUNT = 2;
    
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayerSet> _roomPokerDealingCardsToPlayerSet;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerDealingTimer> _roomPokerDealingTimer;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;
    
    private List<RoomPokerCardNetworkModel> _networkCardsModel;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerActive>()
            .With<RoomPokerCardDesk>()
            .With<RoomPokerDealingCardsToPlayer>()
            .With<RoomPokerDealingCardsToPlayerSet>()
            .Build();
        
        _networkCardsModel = new List<RoomPokerCardNetworkModel>();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
            var dealingCardsTime = config.DealingCardsTime;
            
            DealingCardsAllPlayers(roomEntity, dealingCardsTime);

            _roomPokerDealingTimer.Set(roomEntity,new RoomPokerDealingTimer
            {
                Timer = dealingCardsTime,
            });

            _roomPokerDealingCardsToPlayerSet.Remove(roomEntity);
        }
    }

    private void DealingCardsAllPlayers(Entity roomEntity, float dealingCardsTime)
    {
        ref var roomPokerDealingCardsToPlayer = ref _roomPokerDealingCardsToPlayer.Get(roomEntity);
        ref var pokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

        foreach (var playerEntity in roomPokerDealingCardsToPlayer.QueuePlayers)
        {
            if (playerEntity.IsNullOrDisposed())
            {
                continue;
            }

            _networkCardsModel.Clear();
            var cardsModel = new Queue<CardModel>();

            for (var i = 0; i < HOLDEM_CARD_COUNT; i++)
            {
                if (!pokerCardDesk.CardDesk.TryRandomRemove(out var cardModel))
                {
                    continue;
                }

                cardModel.IsHands = true;
                cardsModel.Enqueue(cardModel);

                _networkCardsModel.Add(new RoomPokerCardNetworkModel
                {
                    Rank = cardModel.Rank,
                    Suit = cardModel.Suit,
                });
            }

            _playerCards.Set(playerEntity, new PlayerCards
            {
                CardsState = CardsState.Close,
                Cards = cardsModel,
            });

            var allPlayerIdsForDealing = new List<int>();

            foreach (var otherPlayerEntity in roomPokerDealingCardsToPlayer.QueuePlayers)
            {
                if (playerEntity.IsNullOrDisposed())
                {
                    continue;
                }

                ref var playerId = ref _playerId.Get(otherPlayerEntity);
                allPlayerIdsForDealing.Add(playerId.Id);
            }

            var dataframe = new RoomPokerDealingCardsByPlayerDataframe
            {
                DealingCardsTime = dealingCardsTime,
                Cards = _networkCardsModel,
                AllPlayersIds = allPlayerIdsForDealing,
            };
            _server.Send(ref dataframe, playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
        _networkCardsModel = null;
    }
}