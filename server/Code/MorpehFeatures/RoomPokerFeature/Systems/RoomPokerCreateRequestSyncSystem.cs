using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.NotificationFeature.Enums;
using server.Code.MorpehFeatures.NotificationFeature.Systems;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCreateRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private RoomPokerStorage _roomPokerStorage;

    [Injectable] private NotificationService _notificationService;

    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerCreateRequestDataframe>(DataframeHandler);
    }

    private void DataframeHandler(RoomPokerCreateRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var player))
        {
            return;
        }
        
        ref var playerCurrency = ref _playerCurrency.Get(player, out var currencyExist);

        if (!currencyExist)
        {
            return;
        }

        if (playerCurrency.CurrencyByType[dataframe.CurrencyType] < dataframe.Contribution)
        {
            _notificationService.Show(player, RoomPokerLocalizationKeys.RoomPokerJoinNotEnoughMoney, NotificationType.Error);
            return;
        }

        _roomPokerStorage.CreateRoom(player, dataframe.MaxPlayers, dataframe.CurrencyType, dataframe.Contribution, 
             dataframe.MinContribution, dataframe.BigBet, dataframe.IsFastMove);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerCreateRequestDataframe>(DataframeHandler);
    }
}