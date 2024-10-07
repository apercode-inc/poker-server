using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
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
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerCardDesk> _pokerCardDesk;
    [Injectable] private Stash<RoomPokerDealingTimer> _pokerDealingTimer;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;

    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;

    private Filter _filter;

    private List<RoomPokerCardNetworkModel> _networkCardsModel;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _networkCardsModel = new List<RoomPokerCardNetworkModel>();

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
            
            roomPokerPlayers.PlayerPotModels.Clear();

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntity = playerBySeat.Value;

                ref var playerId = ref _playerId.Get(playerEntity);
                ref var playerAuthData = ref _playerAuthData.Get(playerEntity);
                
                roomPokerPlayers.PlayerPotModels.Add(new PlayerPotModel(playerAuthData.Guid));

                _networkCardsModel.Clear();

                var cardsModel = new Queue<CardModel>();
                
                for (var i = 0; i < HOLDEM_CARD_COUNT; i++)
                {
                    if (pokerCardDesk.CardDesk.TryRandomRemove(out var cardModel))
                    {
                        cardModel.IsHands = true;
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
                    CardsState = CardsState.Close,
                    Cards = cardsModel,
                });

                var dataframe = new RoomPokerSetCardsByPlayerDataframe
                {
                    PlayerId = playerId.Id,
                    CardsState = CardsState.Close,
                    Cards = _networkCardsModel,
                };
                _server.Send(ref dataframe, playerId.Id);
                
                var dataframeOtherPlayers = new RoomPokerSetCardsByPlayerDataframe
                {
                    PlayerId = playerId.Id,
                    CardsState = CardsState.Close,
                };
                _server.SendInRoomExcept(ref dataframeOtherPlayers, roomEntity, playerEntity);
            }

            _roomPokerSetBlinds.Set(roomEntity, new RoomPokerSetBlinds());

            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
            
            _pokerDealingTimer.Set(roomEntity,new RoomPokerDealingTimer
            {
                Timer = config.DealingCardsTime,
            });
            
            _roomPokerDealingCardsToPlayer.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
        _networkCardsModel = null;
    }
}