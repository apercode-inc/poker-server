using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudSetBetRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerSetBet> _playerSetBet;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private NetFrameServer _server;

    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudSetBetRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudSetBetRequestDataframe dataframe, int clientId)
    {
        if (!_playerStorage.TryGetPlayerById(clientId, out var player))
        {
            return;
        }

        ref var playerRoomPoker = ref _playerRoomPoker.Get(player, out var roomExist);

        if (!roomExist)
        {
            return;
        }

        var roomEntity = playerRoomPoker.RoomEntity;

        ref var playerSeat = ref _playerSeat.Get(player);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        if (playerSeat.SeatIndex != roomPokerPlayers.MoverSeatPointer)
        {
            return;
        }

        if (!IsPlayerBetValid(player, roomEntity, dataframe.Bet))
        {
            ref var authData = ref _playerAuthData.Get(player);
            Logger.LogWarning($"Player sent not valid bet! Guid: {authData.Guid}, bet: {dataframe.Bet}");
            return;
        }
        
        _playerSetBet.Set(player, new PlayerSetBet
        {
            Bet = dataframe.Bet,
        });
        _playerTurnTimerReset.Set(player);
    }

    private bool IsPlayerBetValid(Entity playerEntity, Entity roomEntity, long playerBet)
    {
        ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity, out bool exist);
        if (!exist || playerPokerContribution.Value < playerBet)
        {
            return false;
        }

        ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
        ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
        ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

        if (roomPokerStats.BigBet <= 0)
        {
            Logger.LogWarning("Room big bet is 0. This should never happen");
            return false;
        }

        if (playerBet == playerPokerContribution.Value)
        {
            return true;
        }

        var requiredBet = roomPokerMaxBet.Value - playerPokerCurrentBet.Value;
        var remainderAfterCall = playerPokerContribution.Value - requiredBet;

        if (requiredBet > 0 && remainderAfterCall == 0)
        {
            requiredBet -= playerPokerContribution.Value;
        }
        
        if (playerBet == requiredBet && requiredBet > 0)
        {
            return true;
        }
        
        var raiseBet = requiredBet;
        while (playerPokerContribution.Value > raiseBet)
        {
            raiseBet += roomPokerStats.BigBet;
            if (raiseBet == playerBet)
                return true;
        }
        
        return false;
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudSetBetRequestDataframe>(Handler);
    }
}