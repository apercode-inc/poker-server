using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerListRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    [Injectable] private NetFrameServer _server;

    [Injectable] private RoomPokerStorage _roomPokerStorage;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomsListRequestDataframe>(Handler);

        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerStats>()
            .Without<RoomPokerReadyDestroy>()
            .Build();
    }

    private void Handler(RoomsListRequestDataframe dataframe, int id)
    {
        if (_filter.IsEmpty())
        {
            return;
        }

        var responseDataframe = new RoomsListResponseDataframe
        {
            Rooms = new List<RoomNetworkModel>(),
        };

        foreach (var entity in _filter)
        {
            ref var roomPokerId = ref _roomPokerId.Get(entity);
            ref var roomPokerStats = ref _roomPokerStats.Get(entity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);

            var playersInRoom = new List<RoomPlayerNetworkModel>();
            
            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }

                ref var playerSeat = ref _playerSeat.Get(player);
                ref var playerNickname = ref _playerNickname.Get(player);
                
                playersInRoom.Add(new RoomPlayerNetworkModel
                {
                    Nickname = playerNickname.Value,
                    Seat = playerSeat.SeatIndex,
                });
            }
            
            responseDataframe.Rooms.Add(new RoomNetworkModel
            {
                Id = roomPokerId.Value,
                CurrentPlayers = (byte) roomPokerPlayers.TotalPlayersCount,
                MaxPlayers = roomPokerStats.MaxPlayers,
                SmallBet = roomPokerStats.BigBet / 2,
                BigBet = roomPokerStats.BigBet,
                Players = playersInRoom,
            });
        }
        
        _server.Send(ref responseDataframe, id);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomsListRequestDataframe>(Handler);
        _filter = null;
    }
}