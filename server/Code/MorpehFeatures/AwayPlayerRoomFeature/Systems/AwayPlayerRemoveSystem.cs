using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerRemoveSystem : ISystem
{
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAwayRemove>()
            .With<PlayerAway>()
            .With<PlayerRoomPoker>()
            .With<PlayerId>()
            .With<PlayerSeat>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerAway.Remove(playerEntity);
            _playerAwayRemove.Remove(playerEntity);

            ref var playerId = ref _playerId.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerSeat = ref _playerSeat.Get(playerEntity);
            
            var roomEntity = playerRoomPoker.RoomEntity;
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            roomPokerPlayers.MarkedPlayersBySeat.TryGetValueByMarkers(playerSeat.SeatIndex, out var playerByMarkedItem);
            playerByMarkedItem.IsMoveIgnore = false;
            
            var dataframe = new AwayPlayerResetTimerDataframe
            {
                PlayerId = playerId.Id,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}