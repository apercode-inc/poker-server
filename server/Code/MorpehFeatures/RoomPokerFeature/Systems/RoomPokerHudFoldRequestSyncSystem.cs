using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudFoldRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;

    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerActive> _playerActive;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private RoomPokerService _roomPokerService;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudFoldRequestDataframe>(Handler);
    }
    
    private void Handler(RoomPokerHudFoldRequestDataframe dataframe, int clientId)
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

        if (_roomPokerCleanupTimer.Has(roomEntity) || _roomPokerNextDealingTimer.Has(roomEntity))
        {
            return;
        }

        if (!_playerActive.Has(player))
        {
            return;
        }

        _playerDropCards.Set(player);
        _playerTurnTimerReset.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudFoldRequestDataframe>(Handler);
    }
}