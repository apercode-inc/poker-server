using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

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

        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        roomPokerPlayers.MarkedPlayersBySeat.TryGetValueByMarked(PokerPlayerMarkerType.ActivePlayer,
            out var playerByMarker);
        
        ref var playerAuthData = ref _playerAuthData.Get(player);
        
        if (roomPokerPlayers.PlayerPotModels.TryGetValue(playerAuthData.Guid, out var playerPotModel))
        {
            playerPotModel.SetFold();
        }

        if (playerByMarker.Value != player)
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