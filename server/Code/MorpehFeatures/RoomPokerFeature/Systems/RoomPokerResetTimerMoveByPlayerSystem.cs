using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerResetTimerMoveByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerMoveTimer> _playerMoveTimer;
    [Injectable] private Stash<PlayerMoveTimerReset> _playerMoveTimerReset;
    
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerMoveTimerReset>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            _playerMoveTimerReset.Remove(entity);

            if (!_playerMoveTimer.Has(entity))
            {
                continue;
            }

            ref var playerId = ref _playerId.Get(entity);
            ref var playerRoom = ref _playerRoomPoker.Get(entity);
            
            var dataframe = new RoomPokerResetMoveTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, playerRoom.RoomEntity);

            _playerMoveTimer.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}