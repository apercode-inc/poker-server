using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerAdsRewardedVideoState> _playerAdsRewardedVideoState;
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private ConfigsService _configsService;
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    
    public World World { get; set; }
    
    public void OnAwake()
    {
        _server.Subscribe<AdsRewardedVideoStartShowDataframe>(OnStartShowRewardedVideo);
        _server.Subscribe<AdsRewardedVideoResultDataframe>(OnRewardedVideoResult);
    }

    private void OnStartShowRewardedVideo(AdsRewardedVideoStartShowDataframe dataframe, int sender)
    {
        if (!_playerStorage.TryGetPlayerById(sender, out var playerEntity))
        {
            return;
        }

        ref var state = ref _playerAdsRewardedVideoState.Get(playerEntity, out bool exist);
        if (!exist || state.Value == AdsPlayerState.Wait)
        {
            _playerAdsRewardedVideoState.Set(playerEntity, new PlayerAdsRewardedVideoState
            {
                Value = AdsPlayerState.Show,
            });
        }
    }

    private void OnRewardedVideoResult(AdsRewardedVideoResultDataframe dataframe, int sender)
    {
        if (!_playerStorage.TryGetPlayerById(sender, out var playerEntity))
        {
            return;
        }
        
        var config = _configsService.GetConfig<AdsConfig>(ConfigsPath.Ads);
        ref var state = ref _playerAdsRewardedVideoState.Get(playerEntity, out bool exist);
        if (exist && state.Value == AdsPlayerState.Show && dataframe.IsComplete)
        {
            foreach (var rewardConfig in config.AdsShowRewards)
            {
                if (rewardConfig.Amount > 0)
                {
                    _currencyPlayerService.Give(playerEntity, rewardConfig.CurrencyType, rewardConfig.Amount);
                }
            }
        }
        
        _playerAdsRewardedVideoState.Set(playerEntity, new PlayerAdsRewardedVideoState
        {
            Value = AdsPlayerState.Cooldown,
        });
        _playerAdsRewardedVideoCooldown.Set(playerEntity, new PlayerAdsRewardedVideoCooldown
        {
            Timer = config.RewardedAdsShowCooldown,
        });
    }

    public void Dispose() { }
}