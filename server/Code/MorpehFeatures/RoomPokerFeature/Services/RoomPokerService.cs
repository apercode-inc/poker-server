using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
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
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;

    public World World { get; set; }

    private Dictionary<PokerPlayerMarkerType, Entity> _markersByPlayer;

    public void OnAwake()
    {
        _markersByPlayer = new Dictionary<PokerPlayerMarkerType, Entity>();
    }

    public void AddPlayerIdRoom(Entity roomEntity, Entity playerJoin)
    {
        
    }

    public void RemovePlayerFromRoom(Entity roomEntity, Entity playerLeft)
    {
        ref var roomPokerId = ref _roomPokerId.Get(roomEntity);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
        var markedPlayersBySeat = roomPokerPlayers.MarkedPlayersBySeat;

        _markersByPlayer.Clear();
        var isRemove = markedPlayersBySeat.Remove(playerLeft, _markersByPlayer);

        _roomPokerCardDeskService.ReturnCardInDesk(roomEntity, playerLeft);

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

        var dataframe = new RoomPokerLeftResponseDataframe
        {
            RoomId =  roomPokerId.Value,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, roomEntity);

        _playerDealer.Remove(playerLeft);
        _playerCards.Remove(playerLeft);
        _playerRoomPoker.Remove(playerLeft);
        _playerSeat.Remove(playerLeft);
        _playerPokerContribution.Remove(playerLeft);
        
        if (markedPlayersBySeat.Count != 0)
        {
            return;
        }

        _roomPokerStorage.Remove(roomPokerId.Value);
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