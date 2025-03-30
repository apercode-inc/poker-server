using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Move;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudFoldRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;

    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerMoveTimerReset> _playerMoveTimerReset;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
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

        ref var playerSeat = ref _playerSeat.Get(player);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        if (playerSeat.SeatIndex != roomPokerPlayers.MoverSeatPointer)
        {
            return;
        }

        _playerDropCards.Set(player);
        _playerMoveTimerReset.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudFoldRequestDataframe>(Handler);
    }
}