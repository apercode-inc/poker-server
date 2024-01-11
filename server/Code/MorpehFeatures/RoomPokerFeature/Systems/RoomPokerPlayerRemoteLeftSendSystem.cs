using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerRemoteLeftSendSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomRemoteLeftSend> _playerRoomRemoteLeftSend;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomRemoteLeftSend>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var playerRoomRemoteLeftSend = ref _playerRoomRemoteLeftSend.Get(entity);
            var roomId = playerRoomRemoteLeftSend.RoomId;
            var playerId = playerRoomRemoteLeftSend.PlayerId;

            var dataframe = new RoomPokerRemotePlayerLeftResponseDataframe
            {
                RoomId = roomId,
                PlayerId = playerId,
            };
            _server.Send(ref dataframe, entity);

            _playerRoomRemoteLeftSend.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}