using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoCheckCooldownSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsDbCooldownModels> _playerAdsDbCooldownModels;

    [Injectable] private NetFrameServer _server;
    [Injectable] private AdsDbService _adsDbService;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAdsRewardedVideoCooldown>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var cooldown = ref _playerAdsRewardedVideoCooldown.Get(entity);
            for (int i = cooldown.TimersByPanelId.Count - 1; i >= 0; i--)
            {
                var timer = cooldown.TimersByPanelId[i];
                timer.Item2 -= deltaTime;
                cooldown.TimersByPanelId[i] = timer;
                
                if (timer.Item2 > 0f)
                {
                    continue;
                }
                
                var dataframe = new AdsSetRewardedVideoSetCooldownDataframe
                {
                    PanelId = timer.Item1,
                    RemainingSeconds = 0f,
                };
                _server.Send(ref dataframe, entity);

                ref var dbModels = ref _playerAdsDbCooldownModels.Get(entity);
                for (int j = 0; j < dbModels.Value.Count; j++)
                {
                    var dbModel = dbModels.Value[j];
                    if (dbModel.panel_id == timer.Item1)
                    {
                        _adsDbService.RemovePlayerAdsCooldownAsync(dbModel).Forget();
                        dbModels.Value.RemoveAt(j);
                        break;
                    }
                }

                cooldown.TimersByPanelId.RemoveAt(i);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}