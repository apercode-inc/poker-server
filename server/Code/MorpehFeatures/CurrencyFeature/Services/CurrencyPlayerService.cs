using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Dataframe;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.CurrencyFeature.Services;

public class CurrencyPlayerService : IInitializer
{
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public bool TryTakeFromContribution(Entity room, Entity player, ulong cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        ref var playerId = ref _playerId.Get(player);
        ref var playerPokerContribution = ref _playerPokerContribution.Get(player, out var exist);

        if (!exist)
        {
            return false;
        }

        var currencyType = playerPokerContribution.CurrencyType;
        
        if (playerPokerContribution.Value < cost)
        {
            return false;
        }

        playerPokerContribution.Value -= cost;
        playerCurrency.CurrencyByType[currencyType] -= cost;
        
        var dataframe = new RoomPokerPlayerSetBetDataframe
        {
            ContributionBalance = playerPokerContribution.Value,
            AllBalance = playerCurrency.CurrencyByType[currencyType],
            Bet = cost,
            PlayerId = playerId.Id,
        };
        _server.SendInRoom(ref dataframe, room);

        Send(player, currencyType, playerCurrency.CurrencyByType[currencyType]);

        return true;
    }

    public bool TryTake(Entity player, CurrencyType type, ulong cost)
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

    public void Give(Entity player, CurrencyType type, ulong cost)
    {
        ref var playerCurrency = ref _playerCurrency.Get(player);
        playerCurrency.CurrencyByType[type] += cost;

        Send(player, type, playerCurrency.CurrencyByType[type]);
    }

    private void Send(Entity player, CurrencyType type, ulong value)
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