using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerTimerTickSystem : ISystem
{
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private PlayerStorage _playerStorage;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAway>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerAway = ref _playerAway.Get(playerEntity);
            playerAway.Timer -= deltaTime;
            
            if (playerAway.Timer > 0)
            {
                continue;
            }

            ref var playerId = ref _playerId.Get(playerEntity);
            
            _playerStorage.Remove(playerId.Id);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}