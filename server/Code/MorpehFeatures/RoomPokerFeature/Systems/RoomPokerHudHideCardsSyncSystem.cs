using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudHideCardsSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerShowOrHideTimer> _playerShowOrHideTimer;

    [Injectable] private NetFrameServer _server;
    
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudHideCardsRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudHideCardsRequestDataframe dataframe, int clientId)
    {
        if (!_playerStorage.TryGetPlayerById(clientId, out var player))
        {
            return;
        }

        ref var playerShowOrHideTimer = ref _playerShowOrHideTimer.Get(player, out var showOrHideTimerExist);

        if (!showOrHideTimerExist)
        {
            return;
        }

        playerShowOrHideTimer.TimeCurrent = playerShowOrHideTimer.TimeMax;
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudHideCardsRequestDataframe>(Handler);
    }
}