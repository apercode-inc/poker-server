using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerRemoteJoinSendSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomRemoteJoinSend> _playerRoomRemoteJoinSend;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerRoomRemoteJoinSend>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var playerRoomRemoteJoinSend = ref _playerRoomRemoteJoinSend.Get(entity);

            var dataframe = new RoomPokerRemotePlayerJoinResponseDataframe
            {
                RoomId = playerRoomRemoteJoinSend.RoomId,
                RemotePlayer = playerRoomRemoteJoinSend.RemotePlayer,
            };
            
            _server.Send(ref dataframe, entity);
            
            _playerRoomRemoteJoinSend.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}