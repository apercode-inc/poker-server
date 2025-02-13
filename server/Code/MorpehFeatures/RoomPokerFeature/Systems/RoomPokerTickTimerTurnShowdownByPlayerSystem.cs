using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTickTimerTurnShowdownByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerTurnShowdownTimer> _playerTurnShowdownTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<Destroy> _destroy;
    [Injectable] private Stash<PlayerTurnShowdownResetTimer> _playerTurnShowdownResetTimer;
    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerAway> _playerAway;
    
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    
    [Injectable] private RoomPokerService _roomPokerService;
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerTurnShowdownTimer>()
            .With<PlayerRoomPoker>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in  _filter)
        {
            ref var playerTurnShowdownTimer = ref _playerTurnShowdownTimer.Get(playerEntity);

            playerTurnShowdownTimer.TimeCurrent += deltaTime;

            if (playerTurnShowdownTimer.TimeCurrent < playerTurnShowdownTimer.TimeMax && !_playerAway.Has(playerEntity))
            {
                continue;
            }

            _playerDropCards.Set(playerEntity);
            
            var closeActivePanelDataframe = new RoomPokerPlayerActiveHudPanelCloseDataframe();
            _server.Send(ref closeActivePanelDataframe, playerEntity);

            _playerTurnShowdownResetTimer.Set(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}