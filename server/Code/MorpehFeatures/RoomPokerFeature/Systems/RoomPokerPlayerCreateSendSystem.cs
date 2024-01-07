using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerCreateSendSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomCreateSend> _playerRoomCreateSend;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerRoomCreateSend>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomCreateSend = ref _playerRoomCreateSend.Get(playerEntity);

            var dataframe = new RoomPokerLocalPlayerCreateResponseDataframe
            {
                RoomId = playerRoomCreateSend.RoomId,
                MaxPlayers = playerRoomCreateSend.MaxPlayers,
                Seat = playerRoomCreateSend.Seat,
            };
            _server.Send(ref dataframe, playerEntity);
            
            _playerRoomCreateSend.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}