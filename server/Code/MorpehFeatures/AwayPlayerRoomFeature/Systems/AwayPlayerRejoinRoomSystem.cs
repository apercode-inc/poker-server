using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.TopUpFeature.Dataframes;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerRejoinRoomSystem : ISystem
{
    [Injectable] private Stash<PlayerAwayRejoinRoom> _playerAwayRejoinRoom;
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerOffline> _playerOffline;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;

    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;

    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerAuthData>()
            .With<PlayerRoomPoker>()
            .With<PlayerAwayRejoinRoom>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerAwayRejoinRoom = ref _playerAwayRejoinRoom.Get(playerEntity);
            
            ref var playerId = ref _playerId.Get(playerEntity);
            
            var newPlayerId = playerAwayRejoinRoom.NewId;
            var oldPlayerId = playerId.Id;

            _playerStorage.Replace(oldPlayerId, newPlayerId, playerEntity);

            var roomEntity = playerRoomPoker.RoomEntity;

            var dataframe = new PlayerChangeIdDataframe
            {
                OldId = oldPlayerId,
                NewId = newPlayerId,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
            
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

            if (playerPokerContribution.Value < roomPokerStats.BigBet)
            {
                var topUpOpenDataframe = new TopUpOpenRequestDataframe();
                _server.Send(ref topUpOpenDataframe, playerEntity);
            }
            else
            {
                _playerAwayRemove.Set(playerEntity);
            }
            
            _playerDbModelRequest.Set(playerEntity);
            
            _playerOffline.Remove(playerEntity);
            _playerAwayRejoinRoom.Remove(playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}