using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
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
            //todo
            Debug.LogError("отправить игроку нотиф о том что он не может присоединится из за того что ему не хватает на взнос");
            return;
        }
            
        _roomPokerStorage.Add(player, dataframe.MaxPlayers, dataframe.CurrencyType, dataframe.Contribution, dataframe.BigBet);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerCreateRequestDataframe>(DataframeHandler);
    }
}