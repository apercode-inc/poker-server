using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerResetTimerTurnShowdownByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerTurnShowdownResetTimer> _playerTurnShowdownResetTimer;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerTurnShowdownTimer> _playerTurnShowdownTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerTurnShowdownResetTimer>()
            .With<PlayerRoomPoker>()
            .With<PlayerTurnShowdownTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            _roomPokerShowdownChoiceCheck.Set(roomEntity);
            _playerTurnShowdownTimer.Remove(playerEntity);
        
            ref var playerId = ref _playerId.Get(playerEntity);
        
            var dataframe = new RoomPokerResetTurnTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
            
            _playerTurnShowdownResetTimer.Remove(playerEntity);
        }
    }
    
    private void ClearTimer(Entity roomEntity, Entity playerEntity)
    {
       
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}