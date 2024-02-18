using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerResetTimerTurnByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerTurnTimer> _playerTurnTimer;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerTurnTimerReset>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            _playerTurnTimerReset.Remove(entity);

            if (!_playerTurnTimer.Has(entity))
            {
                continue;
            }

            ref var playerId = ref _playerId.Get(entity);
            ref var playerRoom = ref _playerRoomPoker.Get(entity);
            
            var dataframe = new RoomPokerResetTurnTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, playerRoom.RoomEntity);

            _playerTurnTimer.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}