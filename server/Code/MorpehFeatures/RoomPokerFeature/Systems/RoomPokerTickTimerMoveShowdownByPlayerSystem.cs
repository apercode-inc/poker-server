using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTickTimerMoveShowdownByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerMoveShowdownTimer> _playerMoveShowdownTimer;
    [Injectable] private Stash<PlayerMoveShowdownResetTimer> _playerMoveShowdownResetTimer;
    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerAway> _playerAway;
    
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerMoveShowdownTimer>()
            .With<PlayerRoomPoker>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in  _filter)
        {
            ref var playerMoveShowdownTimer = ref _playerMoveShowdownTimer.Get(playerEntity);

            playerMoveShowdownTimer.TimeCurrent += deltaTime;

            if (playerMoveShowdownTimer.TimeCurrent < playerMoveShowdownTimer.TimeMax && !_playerAway.Has(playerEntity))
            {
                continue;
            }

            _playerDropCards.Set(playerEntity);
            
            var closeActivePanelDataframe = new RoomPokerPlayerActiveHudPanelCloseDataframe();
            _server.Send(ref closeActivePanelDataframe, playerEntity);

            _playerMoveShowdownResetTimer.Set(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}