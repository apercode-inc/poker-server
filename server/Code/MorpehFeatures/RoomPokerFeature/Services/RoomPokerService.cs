using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Services;

public class RoomPokerService : IInitializer
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;

    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerStorage _roomPokerStorage;

    public World World { get; set; }

    private Dictionary<PokerPlayerMarkerType, Entity> _markersByPlayer;

    public void OnAwake()
    {
        _markersByPlayer = new Dictionary<PokerPlayerMarkerType, Entity>();
    }

    public void SendBetInRoom(Entity roomEntity, Entity playerEntity, ulong betValue)
    {
        ref var playerId = ref _playerId.Get(playerEntity);

        var dataframe = new RoomPokerPlayerSetBetDataframe
        {
            Bet = betValue,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }
    
    public void RemovePlayerFromRoom(Entity roomEntity, Entity playerLeft)
    {
        ref var roomPokerId = ref _roomPokerId.Get(roomEntity);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        _markersByPlayer.Clear();
        var isRemove = markedPlayersBySeat.Remove(playerLeft, _markersByPlayer);

        ReturnCardInDesk(roomEntity, playerLeft);

        if (isRemove)
        {
            foreach (var markerByPlayer in _markersByPlayer)
            {
                var marker = markerByPlayer.Key;
                var nextPlayerMarked = markerByPlayer.Value;

                switch (marker)
                {
                    case PokerPlayerMarkerType.DealerPlayer:
                        SetDealerPlayerMarker(roomEntity, nextPlayerMarked);
                        break;
                    case PokerPlayerMarkerType.ActivePlayer:
                        SetActivePlayerMarker(roomEntity, nextPlayerMarked);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        ref var playerId = ref _playerId.Get(playerLeft);

        var dataframe = new RoomPokerRemotePlayerLeftResponseDataframe
        {
            RoomId =  roomPokerId.Value,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);

        _playerDealer.Remove(playerLeft);
        _playerCards.Remove(playerLeft);
        _playerRoomPoker.Remove(playerLeft);
        _playerSeat.Remove(playerLeft);
        
        if (markedPlayersBySeat.Count != 0)
        {
            return;
        }

        _roomPokerStorage.Remove(roomPokerId.Value);
    }

    private void ReturnCardInDesk(Entity roomEntity, Entity playerLeft)
    {
        ref var playerCards = ref _playerCards.Get(playerLeft, out var cardExist);

        if (cardExist)
        {
            ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

            while (playerCards.Cards.Count > 0)
            {
                var card = playerCards.Cards.Dequeue();
                roomPokerCardDesk.CardDesk.Add(card);
            }
        }
    }

    private void SetDealerPlayerMarker(Entity roomEntity, Entity nextMarkedPlayer)
    {
        _playerDealer.Set(nextMarkedPlayer);

        ref var playerSeat = ref _playerSeat.Get(nextMarkedPlayer);
        
        var dataframe = new RoomPokerSetDealerDataframe
        {
            PlayerSeat = playerSeat.SeatIndex,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }

    //todo dev
    private void SetActivePlayerMarker(Entity roomEntity, Entity nextMarkedPlayer)
    {
        
    }

    public void Dispose()
    {
        _markersByPlayer = null;
    }
}