using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudHideCardsRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerTurnShowdownTimer> _playerTurnShowdownTimer;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudHideCardsRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudHideCardsRequestDataframe dataframe, int playerId)
    {
        if (!_playerStorage.TryGetPlayerById(playerId, out var playerEntity))
        {
            return;
        }

        ref var playerTurnShowdownTimer = ref _playerTurnShowdownTimer.Get(playerEntity, out var exist);

        if (!exist)
        {
            return;
        }
        playerTurnShowdownTimer.TimeCurrent = playerTurnShowdownTimer.TimeMax;
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudHideCardsRequestDataframe>(Handler);
    }
}