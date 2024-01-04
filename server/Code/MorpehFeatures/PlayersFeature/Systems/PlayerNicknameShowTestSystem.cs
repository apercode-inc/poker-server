using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerNicknameShowTestSystem : ISystem
{
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerNickname>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var playerId = ref _playerId.Get(entity);
            ref var playerNickname = ref _playerNickname.Get(entity);
            
            Debug.LogColor($"id = {playerId.Id} nickname = {playerNickname.Value}", ConsoleColor.Yellow);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}