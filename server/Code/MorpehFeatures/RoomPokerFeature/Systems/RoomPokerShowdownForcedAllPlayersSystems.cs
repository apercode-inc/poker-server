using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownForcedAllPlayersSystems : ISystem
{
    [Injectable] private Stash<RoomPokerShowdownForcedAllPlayersDone> _roomPokerShowdownForcedAllPlayersDone;
    [Injectable] private Stash<RoomPokerShowdownForcedAllPlayers> _roomPokerShowdownForcedAllPlayers;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private Stash<PlayerShowdownForced> _playerShowdownForced;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerShowdownForcedAllPlayers>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }

                _playerShowdownForced.Set(player);
            }
            
            _roomPokerShowdownForcedAllPlayersDone.Set(roomEntity);
            _roomPokerShowdownForcedAllPlayers.Remove(roomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}