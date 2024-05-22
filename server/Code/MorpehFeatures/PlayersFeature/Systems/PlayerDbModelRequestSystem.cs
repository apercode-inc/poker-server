using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.ThreadSafeContainers;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerDbModelRequestSystem : ISystem
{
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    
    [Injectable] private ThreadSafeFilter<PlayerDbModelThreadSafe> _threadSafeFilter;

    [Injectable] private PlayerDbService _playerDbService;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerDbModelRequest>()
            .With<PlayerAuthData>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerAuthData = ref _playerAuthData.Get(playerEntity);
            var playerGuid = playerAuthData.Guid;

            Task.Run(async () =>
            {
                var model = await _playerDbService.GetPlayerAsync(playerGuid);
                
                if (model.Any())
                {
                    _threadSafeFilter.Add(new PlayerDbModelThreadSafe
                    {
                        Player = playerEntity,
                        Model = model.First(),
                    });
                }
            });
            
            _playerDbModelRequest.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}