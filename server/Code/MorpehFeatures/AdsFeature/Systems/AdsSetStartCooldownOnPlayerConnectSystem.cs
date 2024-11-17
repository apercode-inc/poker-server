using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsSetStartCooldownOnPlayerConnectSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerInitializeAds> _playerInitializeAds;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerInitializeAds>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        if (_filter.IsEmpty()) return;
        
        foreach (var entity in _filter)
        {
            _playerAdsRewardedVideoCooldown.Set(entity, new PlayerAdsRewardedVideoCooldown
            {
                TimersByPanelId = new List<(string, float)>(),
            });

            _playerInitializeAds.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}