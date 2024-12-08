using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Configs;
using server.Code.MorpehFeatures.AdsFeature.Dataframes;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;

namespace server.Code.MorpehFeatures.AdsFeature.Systems;

public class AdsRewardedVideoRewardsSyncSystem : IInitializer
{
    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RewardedAdPanelRewardsRequestDataframe>(Handler);
    }

    private void Handler(RewardedAdPanelRewardsRequestDataframe dataframe, int sender)
    {
        var config = _configsService.GetConfig<AdsConfig>(ConfigsPath.Ads);
        if (!config.RewardsForPanels.TryGetValue(dataframe.PanelId, out var panelConfig))
        {
            return;
        }
        
        var rewardsList = new List<RewardItemDataframe>();
        foreach (var adsShowReward in panelConfig.AdsShowRewards)
        {
            rewardsList.Add(new RewardItemDataframe
            {
                CurrencyType = adsShowReward.CurrencyType,
                Count = adsShowReward.Amount,
            });
        }

        var response = new RewardedAdPanelRewardsListResponseDataframe
        {
            PanelId = dataframe.PanelId,
            Rewards = rewardsList,
        };
        _server.Send(ref response, sender);
    }

    public void Dispose()
    {
    }
}