using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerJoinSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayerJoin> _roomPokerPlayerJoin;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

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
            
            roomPokerPlayers.Players.Add(roomPokerPlayerJoin.Player);

            _roomPokerPlayerJoin.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}