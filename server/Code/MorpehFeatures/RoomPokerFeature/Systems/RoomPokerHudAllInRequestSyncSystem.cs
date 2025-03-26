using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudAllInRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerSetBet> _playerSetBet;
    [Injectable] private Stash<PlayerActive> _playerActive;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudAllInRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudAllInRequestDataframe dataframe, int clientId)
    {
        if (!_playerStorage.TryGetPlayerById(clientId, out var player))
        {
            return;
        }

        if (!_playerRoomPoker.Has(player))
        {
            return;
        }

        if (!_playerActive.Has(player))
        {
            return;
        }

        ref var playerPokerContribution = ref _playerPokerContribution.Get(player);
        
        _playerSetBet.Set(player, new PlayerSetBet
        {
            Bet = playerPokerContribution.Value,
        });
        _playerTurnTimerReset.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudAllInRequestDataframe>(Handler);
    }
}