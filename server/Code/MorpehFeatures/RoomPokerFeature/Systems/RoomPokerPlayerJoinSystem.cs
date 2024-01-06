using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerJoinSystem : ISystem
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerPlayerJoin> _roomPokerPlayerJoin;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayerJoin>()
            .With<RoomPokerStats>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var roomPokerStats = ref _roomPokerStats.Get(entity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);

            if (roomPokerStats.MaxPlayers == roomPokerPlayers.Players.Count)
            {
                Debug.LogError($"[RoomPokerPlayerJoinSystem.OnUpdate] trying to enter a crowded room");
                continue;
            }

            ref var roomPokerPlayerJoin = ref _roomPokerPlayerJoin.Get(entity);

            if (roomPokerPlayers.Players.ContainsKey(roomPokerPlayerJoin.Player))
            {
                continue;
            }
            
            ref var roomPokerId = ref _roomPokerId.Get(entity);

            byte seat = 0;

            while (roomPokerPlayers.Players.ContainsValue(seat))
            {
                seat++;
            }
            
            roomPokerPlayers.Players.Add(roomPokerPlayerJoin.Player, seat);
            
            ref var playerRoomPoker = ref _playerRoomPoker.Get(roomPokerPlayerJoin.Player, out var exist);

            if (exist)
            {
                playerRoomPoker.RoomIds.Add(roomPokerId.Value);
            }
            else
            {
                _playerRoomPoker.Set(roomPokerPlayerJoin.Player, new PlayerRoomPoker
                {
                    RoomIds = new List<int> {roomPokerId.Value},
                });
            }

            _roomPokerPlayerJoin.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}