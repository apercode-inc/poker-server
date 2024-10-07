using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Dataframe;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.CurrencyFeature.Services;

public class CurrencyPlayerService : IInitializer
{
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerDbEntry> _playerDbEntry;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerAllin> _playerAllin;

    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerDbService _playerDbService;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public bool TryGiveBank(Entity room, Entity player, long cost)
    {
        ref var roomPokerBank = ref _roomPokerBank.Get(room);

        if (roomPokerBank.OnTable <= 0)
        {
            return false;
        }

        ref var playerCurrency = ref _playerCurrency.Get(player);
        ref var playerId = ref _playerId.Get(player);
        ref var playerPokerContribution = ref _playerPokerContribution.Get(player);

        var currencyType = playerPokerContribution.CurrencyType;

        playerPokerContribution.Value += cost;
        var newBalance = playerCurrency.CurrencyByType[currencyType] += cost;

        SetInDatabase(player, currencyType, newBalance);

        var dataframe = new RoomPokerPlayerGiveBankDataframe
        {
            ContributionBalance = playerPokerContribution.Value,
            AllBalance = playerCurrency.CurrencyByType[currencyType],
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, room);

        Send(player, currencyType, playerCurrency.CurrencyByType[currencyType]);
        
        return true;
    }

    public bool TrySetBet(Entity room, Entity player, long cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        ref var playerId = ref _playerId.Get(player);
        ref var playerPokerContribution = ref _playerPokerContribution.Get(player);

        if (playerPokerContribution.Value < cost)
        {
            return false;
        }
        
        ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
        ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(room);

        if (playerPokerCurrentBet.Value + cost < roomPokerMaxBet.Value)
        {
            return false;
        }
        
        var currencyType = playerPokerContribution.CurrencyType;

        playerPokerContribution.Value -= cost;
        var newBalance = playerCurrency.CurrencyByType[currencyType] -= cost;
        
        SetInDatabase(player, currencyType, newBalance);

        playerPokerCurrentBet.Value += cost;
        
        var dataframe = new RoomPokerPlayerSetBetDataframe
        {
            ContributionBalance = playerPokerContribution.Value,
            AllBalance = playerCurrency.CurrencyByType[currencyType],
            Bet = playerPokerCurrentBet.Value,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, room);

        ref var playerAuthData = ref _playerAuthData.Get(player);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(room);

        PlayerPotModel targetPlayerPotModel = null;
        foreach (var playerPotModel in roomPokerPlayers.PlayerPotModels)
        {
            if (playerPotModel.Guid != playerAuthData.Guid)
            {
                continue;
            }

            targetPlayerPotModel = playerPotModel;
        }

        if (targetPlayerPotModel == null)
        {
            Logger.Error($"[CurrencyPlayerService.TrySetBet] player pot model not exist collection, guid: {playerAuthData.Guid}");
            return false;
        }
        
        targetPlayerPotModel.SetBet(cost);
            
        if (playerPokerContribution.Value == 0)
        {
            _playerAllin.Set(player);
        }

        ref var roomPokerBank = ref _roomPokerBank.Get(room);
        
        roomPokerBank.Total += cost;

        if (roomPokerMaxBet.Value < playerPokerCurrentBet.Value)
        {
            roomPokerMaxBet.Value = playerPokerCurrentBet.Value;
        }
        
        Send(player, currencyType, playerCurrency.CurrencyByType[currencyType]);

        return true;
    }

    public bool TryTake(Entity player, CurrencyType type, long cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);

        if (playerCurrency.CurrencyByType[type] < cost)
        {
            return false;
        }
        var newBalance = playerCurrency.CurrencyByType[type] -= cost;
        
        SetInDatabase(player, type, newBalance);
        Send(player, type, playerCurrency.CurrencyByType[type]);
        
        return true;
    }

    public void Give(Entity player, CurrencyType type, long cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        var newBalance = playerCurrency.CurrencyByType[type] += cost;

        SetInDatabase(player, type, newBalance);
        Send(player, type, playerCurrency.CurrencyByType[type]);
    }
    
    private void SetInDatabase(Entity player, CurrencyType currencyType, long newBalance)
    {
        ref var playerDbEntry = ref _playerDbEntry.Get(player);
        switch (currencyType)
        {
            case CurrencyType.Chips:
                playerDbEntry.Model.chips = newBalance;
                _playerDbService.UpdateChipsPlayerThreadPool(playerDbEntry.Model.unique_id, newBalance).Forget();
                break;
            case CurrencyType.Gold:
                playerDbEntry.Model.gold = newBalance;
                break;
            case CurrencyType.Stars:
                playerDbEntry.Model.stars = newBalance;
                break;
        }
    }

    private void Send(Entity player, CurrencyType type, long value)
    {
        var dataframe = new CurrencyUpdateDataframe
        {
            Type = type,
            Value = value,
        };
        _server.Send(ref dataframe, player);
    }

    public void Dispose()
    {
    }
}