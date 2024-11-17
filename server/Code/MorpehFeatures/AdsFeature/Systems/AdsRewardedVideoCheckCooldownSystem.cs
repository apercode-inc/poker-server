using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoCheckCooldownSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private NetFrameServer _server;

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
                
                if (timer.Item2 > 0f) continue;
                
                ref var playerId = ref _playerId.Get(entity);
                var dataframe = new AdsSetRewardedVideoSetCooldownDataframe
                {
                    PanelId = timer.Item1,
                    OnCooldown = false
                };
                _server.Send(ref dataframe, playerId.Id);
                cooldown.TimersByPanelId.RemoveAt(i);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}