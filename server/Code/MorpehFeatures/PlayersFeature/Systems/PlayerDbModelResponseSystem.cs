using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.ThreadSafeContainers;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerDbModelResponseSystem : ISystem
{
    [Injectable] private Stash<PlayerDbEntry> _playerDbEntry;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerInitialize> _playerInitialize;
    
    [Injectable] private ThreadSafeFilter<PlayerDbModelThreadSafe> _threadSafeFilter;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var container in _threadSafeFilter)
        {
            var playerEntity = container.Player;

            if (playerEntity.IsNullOrDisposed())
            {
                continue;
            }
            
            _playerInitialize.Set(playerEntity, new PlayerInitialize
            {
                DbPlayerModel = container.Model,
            });
        }
    }

    public void Dispose()
    {
    }
}