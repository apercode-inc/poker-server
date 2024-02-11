using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowTestSystem : ISystem
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerStats>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var roomPokerId = ref _roomPokerId.Get(entity);
            ref var roomPokerStats = ref _roomPokerStats.Get(entity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);
            
            Debug.LogColor($"roomId: {roomPokerId.Value} maxPlayers: {roomPokerStats.MaxPlayers} smallBet: {roomPokerStats.BigBet / 2} bigBet: {roomPokerStats.BigBet} playerCount: {roomPokerPlayers.MarkedPlayersBySeat.Count}", ConsoleColor.Magenta);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}