using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownMoveRequestByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerPokerShowdownMoveRequest> _playerPokerShowdownMoveRequest;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerMoveShowdownTimer> _playerMoveShowdownTimer;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerPokerShowdownMoveRequest>()
            .With<PlayerRoomPoker>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);

            var roomEntity = playerRoomPoker.RoomEntity;
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            
            _playerMoveShowdownTimer.Set(playerEntity, new PlayerMoveShowdownTimer
            {
                TimeCurrent = 0,
                TimeMax = roomPokerStats.MoveShowdownTime,
            });
            
            _playerPokerShowdownMoveRequest.Remove(playerEntity);

            if (_playerAway.Has(playerEntity))
            {
                continue;
            }
            
            ref var playerId = ref _playerId.Get(playerEntity);
            
            var timeDataframe = new RoomPokerSetTimerMoveDataframe
            {
                PlayerId = playerId.Id,
                Time = roomPokerStats.MoveShowdownTime,
            };
            _server.SendInRoom(ref timeDataframe, roomEntity);
            
            var dataframe = new RoomPokerPlayerMoveRequestDataframe
            {
                MoveType = PokerPlayerMoveType.Showdown,
            };
            _server.Send(ref dataframe, playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}