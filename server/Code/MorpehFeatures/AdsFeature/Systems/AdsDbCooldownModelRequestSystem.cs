using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.ThreadSafeContainers;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsDbCooldownModelRequestSystem : ISystem
{
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerAdsDbCooldownModelRequest> _playerAdsDbCooldownModelRequest;

    [Injectable] private ThreadSafeFilter<PlayerAdsCooldownDbModelThreadSafe> _playerAdsCooldownDbModelThreadSafe;

    [Injectable] private AdsDbService _adsDbService;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAuthData>()
            .With<PlayerAdsDbCooldownModelRequest>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var authData = ref _playerAuthData.Get(entity);
            var playerGuid = authData.Guid;

            Task.Run(async () =>
            {
                var models = await _adsDbService.GetPlayerAdsCooldownsAsync(playerGuid);
                _playerAdsCooldownDbModelThreadSafe.Add(new PlayerAdsCooldownDbModelThreadSafe
                {
                    Player = entity,
                    AdsCooldownModels = models.ToList(),
                });
            });

            _playerAdsDbCooldownModelRequest.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}