using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCreateOrJoinSendSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomCreateSend> _playerRoomCreateSend;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomCreateSend>()
            .With<PlayerRoomPoker>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var requestingPlayer in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(requestingPlayer);
            var roomEntity = playerRoomPoker.RoomEntity;

            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerId = ref _roomPokerId.Get(roomEntity);

            var roomPlayerNetworkModels = new List<RoomPlayerNetworkModel>();
            RoomPlayerNetworkModel thisPlayerModel = default;
            
            foreach (var playersBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntityFromRoom = playersBySeat.Value;

                var thisPlayer = playerEntityFromRoom == requestingPlayer;

                ref var playerId = ref _playerId.Get(playerEntityFromRoom);
                ref var playerNickname = ref _playerNickname.Get(playerEntityFromRoom);
                var isDealer = _playerDealer.Has(playerEntityFromRoom);
                ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntityFromRoom);
                ref var playerCurrency = ref _playerCurrency.Get(playerEntityFromRoom);
                ref var playerCards = ref _playerCards.Get(playerEntityFromRoom);
                ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntityFromRoom);

                var cardsModel = new List<RoomPokerCardNetworkModel>();

                if (thisPlayer || playerCards.CardsState == CardsState.Open)
                {
                    foreach (var card in playerCards.Cards)
                    {
                        cardsModel.Add(new RoomPokerCardNetworkModel
                        {
                            Rank = card.Rank,
                            Suit = card.Suit,
                        });
                    }
                }

                var playerNetworkModel = new RoomPlayerNetworkModel
                {
                    Id = playerId.Id,
                    Nickname = playerNickname.Value,
                    Seat = (byte) playersBySeat.Key,
                    IsDealer = isDealer,
                    ContributionBalance = playerPokerContribution.Value,
                    AllBalance = playerCurrency.CurrencyByType[roomPokerStats.CurrencyType],
                    CurrentBet = playerPokerCurrentBet.Value,
                    CardsState = playerCards.CardsState,
                    CardsModel = cardsModel,
                };
                roomPlayerNetworkModels.Add(playerNetworkModel);

                if (thisPlayer)
                {
                    thisPlayerModel = playerNetworkModel;
                }
            }

            var createDataframe = new RoomPokerCreateResponseDataframe
            {
                RoomId = roomPokerId.Value,
                MaxPlayers = roomPokerStats.MaxPlayers,
                CurrencyType = roomPokerStats.CurrencyType,
                PlayerModels = roomPlayerNetworkModels,
            };
            _server.Send(ref createDataframe, requestingPlayer);

            thisPlayerModel.CardsModel?.Clear();
            
            var joinDataframe = new RoomPokerJoinResponseDataframe
            {
                RoomId = roomPokerId.Value,
                PlayerModel = thisPlayerModel,
            };
            _server.SendInRoomExcept(ref joinDataframe, roomEntity, requestingPlayer);
            
            _playerRoomCreateSend.Remove(requestingPlayer);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}