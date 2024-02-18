using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudCheckRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerPokerCheck> _playerPokerCheck;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudCheckRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudCheckRequestDataframe dataframe, int clientId)
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

        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        roomPokerPlayers.MarkedPlayersBySeat.TryGetValueByMarked(PokerPlayerMarkerType.ActivePlayer,
            out var playerByMarker);

        if (playerByMarker.Value != player)
        {
            return;
        }

        ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
        ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);

        if (roomPokerMaxBet.Value - playerPokerCurrentBet.Value > 0)
        {
            return;
        }
        
        _playerPokerCheck.Set(player);
        _playerTurnTimerReset.Set(player);

    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudCheckRequestDataframe>(Handler);
    }
}