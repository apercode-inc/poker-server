using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerLocalJoinSendSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomLocalJoinSend> _playerRoomLocalJoinSend;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerRoomLocalJoinSend>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var playerRoomLocalJoinSend = ref _playerRoomLocalJoinSend.Get(entity);

            var dataframe = new RoomPokerLocalPlayerJoinResponseDataframe
            {
                RoomId = playerRoomLocalJoinSend.RoomId,
                MaxPlayers = playerRoomLocalJoinSend.MaxPlayers,
                Seat = playerRoomLocalJoinSend.Seat,
                RemotePlayers = playerRoomLocalJoinSend.RemotePlayers,
            };
            _server.Send(ref dataframe, entity);
            
            var startGameWaitTime = playerRoomLocalJoinSend.WaitTime;

            if (startGameWaitTime > 0)
            {
                var timerDataframe = new PokerStartGameSetTimerDataframe
                {
                    WaitTime = startGameWaitTime,
                };
                _server.Send(ref timerDataframe, entity);
            }
            
            _playerRoomLocalJoinSend.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}