using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerResetTimerMoveShowdownByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerMoveShowdownResetTimer> _playerMoveShowdownResetTimer;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerMoveShowdownTimer> _playerMoveShowdownTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerMoveShowdownResetTimer>()
            .With<PlayerRoomPoker>()
            .With<PlayerMoveShowdownTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            _roomPokerShowdownChoiceCheck.Set(roomEntity);
            _playerMoveShowdownTimer.Remove(playerEntity);
        
            ref var playerId = ref _playerId.Get(playerEntity);
        
            var dataframe = new RoomPokerResetMoveTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
            
            _playerMoveShowdownResetTimer.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}