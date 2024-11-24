using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.AdsFeature.Configs;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerAdsRewardedVideoCooldown> _playerAdsRewardedVideoCooldown;
    [Injectable] private Stash<PlayerAdsDbCooldownModels> _playerAdsDbCooldownModels;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private ConfigsService _configsService;
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    
    public World World { get; set; }
    
    public void OnAwake()
    {
        _server.Subscribe<AdsRewardedVideoResultDataframe>(OnRewardedVideoResult);
    }

    private void OnRewardedVideoResult(AdsRewardedVideoResultDataframe dataframe, int sender)
    {
        if (!_playerStorage.TryGetPlayerById(sender, out var playerEntity))
        {
            return;
        }
        
        var config = _configsService.GetConfig<AdsConfig>(ConfigsPath.Ads);
        if (!config.RewardsForPanels.TryGetValue(dataframe.PanelId, out var panelConfig))
        {
            return;
        }
        
        bool onCooldown = IsAdPanelOnCooldown(dataframe.PanelId, playerEntity);
        if (dataframe.IsCompleted && !onCooldown)
        {
            GivePlayerRewardByPanelId(playerEntity, panelConfig);
        }
        
        SetAdPanelCooldown(dataframe.PanelId, playerEntity, panelConfig.RewardedAdsShowCooldown);
        
        var setCooldownDataframe = new AdsSetRewardedVideoSetCooldownDataframe
        {
            PanelId = dataframe.PanelId,
            RemainingSeconds = panelConfig.RewardedAdsShowCooldown,
        };
        _server.Send(ref setCooldownDataframe, sender);
    }

    private void GivePlayerRewardByPanelId(Entity playerEntity, AdsConfigById rewards)
    {
        foreach (var rewardConfig in rewards.AdsShowRewards)
        {
            if (rewardConfig.Amount > 0)
            {
                _currencyPlayerService.Give(playerEntity, rewardConfig.CurrencyType, rewardConfig.Amount);
            }
        }
    }

    private bool IsAdPanelOnCooldown(string panelId, Entity playerEntity)
    {
        ref var cooldownPanels = ref _playerAdsRewardedVideoCooldown.Get(playerEntity);
        foreach (var timer in cooldownPanels.TimersByPanelId)
        {
            if (timer.Item1 == panelId)
            {
                return true;
            }
        }

        return false;
    }

    private void SetAdPanelCooldown(string panelId, Entity playerEntity, float cooldown)
    {
        ref var cooldownPanels = ref _playerAdsRewardedVideoCooldown.Get(playerEntity);
        bool foundPanel = false;
        for (int i = 0; i < cooldownPanels.TimersByPanelId.Count; i++)
        {
            var timer = cooldownPanels.TimersByPanelId[i];
            if (timer.Item1 == panelId)
            {
                foundPanel = true;
                timer.Item2 = cooldown;
                cooldownPanels.TimersByPanelId[i] = timer;
            }
        }

        if (!foundPanel)
        {
            cooldownPanels.TimersByPanelId.Add((panelId, cooldown));
        }
    }

    public void Dispose() { }
}