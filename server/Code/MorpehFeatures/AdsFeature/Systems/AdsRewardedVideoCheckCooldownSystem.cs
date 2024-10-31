using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoCheckCooldownSystem : ISystem
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsRewardedVideoState> _playerAdsRewardedVideoState;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAdsRewardedVideoCooldown>()
            .With<PlayerAdsRewardedVideoState>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var cooldown = ref _playerAdsRewardedVideoCooldown.Get(entity);
            cooldown.Timer -= deltaTime;
            if (cooldown.Timer > 0f)
            {
                continue;
            }

            _playerAdsRewardedVideoCooldown.Remove(entity);
            _playerAdsRewardedVideoState.Set(entity, new PlayerAdsRewardedVideoState
            {
                Value = AdsPlayerState.Wait
            });

            ref var playerId = ref _playerId.Get(entity);
            var dataframe = new AdsSetShowRewardedVideoDataframe();
            _server.Send(ref dataframe, playerId.Id);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}