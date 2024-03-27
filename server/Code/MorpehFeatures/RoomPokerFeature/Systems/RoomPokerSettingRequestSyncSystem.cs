using System.ComponentModel;
using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSettingRequestSyncSystem : IInitializer
{
    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerSettingsRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerSettingsRequestDataframe dataframe, int clientId)
    {
        var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);

        var betNetworkModels = new List<RoomPokerSettingsBetNetworkModel>();

        foreach (var bet in config.Bets)
        {
            betNetworkModels.Add(new RoomPokerSettingsBetNetworkModel
            {
                BlindBig = bet.BlindBig,
                Contribution = bet.Contribution,
            });
        }

        var responseDataframe = new RoomPokerSettingsResponseDataframe
        {
            SeatCounts = config.SeatCounts,
            Bets = betNetworkModels,
        };
        _server.Send(ref responseDataframe, clientId);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerSettingsRequestDataframe>(Handler);
    }
}