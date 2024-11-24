using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.DbModels;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.GameTimeFeature;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsSetPlayerCooldownTimersInDbOnLeaveSystem : ICleanupSystem
{
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsDbCooldownModels> _playerAdsDbCooldownModels;

    [Injectable] private GameTimeService _gameTimeService;
    [Injectable] private AdsDbService _adsDbService;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAuthData>()
            .With<PlayerAdsRewardedVideoCooldown>()
            .With<PlayerAdsDbCooldownModels>()
            .With<Destroy>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var cooldowns = ref _playerAdsRewardedVideoCooldown.Get(entity);
            foreach (var timer in cooldowns.TimersByPanelId)
            {
                if (timer.Item2 <= DbPlayerAdsCooldownConstants.WriteCooldownInDbTimerThreshold)
                {
                    continue;
                }

                ref var cooldownModels = ref _playerAdsDbCooldownModels.Get(entity);
                bool foundPanel = false;
                int endTimestamp = _gameTimeService.CurrentTimeStamp + (int)timer.Item2;

                foreach (var dbModel in cooldownModels.Value)
                {
                    if (dbModel.panel_id == timer.Item1)
                    {
                        dbModel.end_timestamp = endTimestamp;
                        _adsDbService.UpdatePlayerAdsCooldownAsync(dbModel).Forget();
                        foundPanel = true;
                    }
                }

                if (!foundPanel)
                {
                    ref var authData = ref _playerAuthData.Get(entity);
                    var dbModel = new DbPlayerAdsCooldownModel
                    {
                        player_id = authData.Guid,
                        panel_id = timer.Item1,
                        end_timestamp = endTimestamp
                    };
                    _adsDbService.InsertPlayerAdsCooldownAsync(dbModel).Forget();
                    cooldownModels.Value.Add(dbModel);
                }
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}