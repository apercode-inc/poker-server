using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTickTimerShowOrHideByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerShowOrHideTimer> _playerShowOrHideTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;

    [Injectable] private RoomPokerService _roomPokerService;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerRoomPoker>()
            .With<PlayerShowOrHideTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerShowOrHideTimer = ref _playerShowOrHideTimer.Get(playerEntity);
            
            playerShowOrHideTimer.TimeCurrent += deltaTime;
            
            if (playerShowOrHideTimer.TimeCurrent < playerShowOrHideTimer.TimeMax)
            {
                continue;
            }

            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            _roomPokerService.DropCards(roomEntity, playerEntity, false);
            
            _roomPokerShowOrHideCardsActivate.Set(roomEntity);
            
            var closeActivePanelDataframe = new RoomPokerPlayerActiveHudPanelCloseDataframe();
            _server.Send(ref closeActivePanelDataframe, playerEntity);
            
            ref var playerId = ref _playerId.Get(playerEntity);
            
            var resetTimerDataframe = new RoomPokerResetTurnTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref resetTimerDataframe, roomEntity);

            _playerShowOrHideTimer.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}