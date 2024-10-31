using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsSetStartCooldownOnPlayerConnectSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsRewardedVideoState> _playerAdsRewardedVideoState;

    [Injectable] private ConfigsService _configsService;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerInitialize>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        if (_filter.IsEmpty()) return;
        
        var config = _configsService.GetConfig<AdsConfig>(ConfigsPath.Ads);
        float cooldown = MathF.Max(config.RewardedAdsShowCooldownOnStart, 1f);
        
        foreach (var entity in _filter)
        {
            _playerAdsRewardedVideoCooldown.Set(entity, new PlayerAdsRewardedVideoCooldown
            {
                Timer = cooldown,
            });
            
            _playerAdsRewardedVideoState.Set(entity, new PlayerAdsRewardedVideoState
            {
                Value = AdsPlayerState.Cooldown,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}