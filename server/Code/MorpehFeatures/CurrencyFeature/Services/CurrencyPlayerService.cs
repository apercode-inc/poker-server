using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Dataframe;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.CurrencyFeature.Services;

public class CurrencyPlayerService : IInitializer
{
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public bool TrySetBet(Entity room, Entity player, long cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        ref var playerId = ref _playerId.Get(player);
        ref var playerPokerContribution = ref _playerPokerContribution.Get(player);

        var currencyType = playerPokerContribution.CurrencyType;
        
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

        playerPokerContribution.Value -= cost;
        playerCurrency.CurrencyByType[currencyType] -= cost;

        playerPokerCurrentBet.Value += cost;
        
        var dataframe = new RoomPokerPlayerSetBetDataframe
        {
            ContributionBalance = playerPokerContribution.Value,
            AllBalance = playerCurrency.CurrencyByType[currencyType],
            Bet = playerPokerCurrentBet.Value,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, room);

        ref var roomPokerBank = ref _roomPokerBank.Get(room);

        roomPokerBank.Value += cost;

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
        playerCurrency.CurrencyByType[type] -= cost;
        
        Send(player, type, playerCurrency.CurrencyByType[type]);
        
        return true;
    }

    public void Give(Entity player, CurrencyType type, long cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        playerCurrency.CurrencyByType[type] += cost;

        Send(player, type, playerCurrency.CurrencyByType[type]);
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