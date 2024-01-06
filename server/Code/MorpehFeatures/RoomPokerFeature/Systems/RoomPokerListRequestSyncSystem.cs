using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerListRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private Stash<PlayerNickname> _playerNickname;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private RoomPokerStorageSystem _roomPokerStorage;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomsListRequestDataframe>(Handler);

        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerStats>()
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
            
            foreach (var playerEntity in roomPokerPlayers.Players)
            {
                ref var playerNickname = ref _playerNickname.Get(playerEntity.Key);
                playersInRoom.Add(new RoomPlayerNetworkModel
                {
                    Nickname = playerNickname.Value,
                    Seat = playerEntity.Value,
                });
            }
            
            responseDataframe.Rooms.Add(new RoomNetworkModel
            {
                Id = roomPokerId.Value,
                CurrentPlayers = (byte) roomPokerPlayers.Players.Count,
                MaxPlayers = roomPokerStats.MaxPlayers,
                SmallBet = roomPokerStats.SmallBet,
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