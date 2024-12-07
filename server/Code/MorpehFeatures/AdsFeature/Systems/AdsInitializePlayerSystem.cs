using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.GameTimeFeature;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsInitializePlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsDbCooldownModels> _playerAdsDbCooldownModels;
    [Injectable] private Stash<PlayerInitializeAds> _playerInitializeAds;

    [Injectable] private GameTimeService _gameTimeService;
    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerAdsDbCooldownModels>()
            .With<PlayerInitializeAds>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            var timers = new List<(string, float)>();
            _playerAdsRewardedVideoCooldown.Set(entity, new PlayerAdsRewardedVideoCooldown
            {
                TimersByPanelId = timers,
            });

            int timeStamp = _gameTimeService.CurrentTimeStamp;
            ref var dbModels = ref _playerAdsDbCooldownModels.Get(entity);
            foreach (var adsCooldownModel in dbModels.Value)
            {
                int remainingSeconds = adsCooldownModel.end_timestamp - timeStamp;
                if (remainingSeconds <= 0)
                {
                    continue;
                }
                
                timers.Add((adsCooldownModel.panel_id, remainingSeconds));
                var dataframe = new AdsSetRewardedVideoSetCooldownDataframe
                {
                    PanelId = adsCooldownModel.panel_id,
                    RemainingSeconds = remainingSeconds,
                };
                _server.Send(ref dataframe, entity);
            }

            _playerInitializeAds.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}