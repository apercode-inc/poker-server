using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.ThreadSafeContainers;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsDbCooldownModelResponseSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsDbCooldownModels> _playerAdsDbCooldownModels;
    [Injectable] private Stash<PlayerInitializeAds> _playerInitializeAds;
    
    [Injectable] private ThreadSafeFilter<PlayerAdsCooldownDbModelThreadSafe> _playerAdsCooldownDbModelThreadSafe;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var container in _playerAdsCooldownDbModelThreadSafe)
        {
            var playerEntity = container.Player;
            if (playerEntity.IsNullOrDisposed())
            {
                continue;
            }
            
            _playerAdsDbCooldownModels.Set(playerEntity, new PlayerAdsDbCooldownModels
            {
                Value = container.AdsCooldownModels
            });
            _playerInitializeAds.Set(playerEntity);
        }
    }

    public void Dispose()
    {
    }
}