using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerAllShowdownForcedSetSystem : ISystem
{
    [Injectable] private Stash<RoomPokerAllShowdown> _roomPokerAllShowdown;
    [Injectable] private Stash<RoomPokerAllShowdownSet> _roomPokerAllShowdownSet;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private Stash<PlayerShowdownForced> _playerShowdownForced;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerAllShowdownSet>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntity = playerBySeat.Value;
                _playerShowdownForced.Set(playerEntity);
            }
            
            _roomPokerAllShowdown.Set(roomEntity);
            _roomPokerAllShowdownSet.Remove(roomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}