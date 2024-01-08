using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerLeftSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayerLeft> _roomPokerPlayerLeft;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    
    [Injectable] private Stash<PlayerRoomRemoteLeftSend> _playerRoomRemoteLeftSend;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private RoomPokerStorageSystem _roomPokerStorage;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayerLeft>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);
            ref var roomPokerPlayerLeft = ref _roomPokerPlayerLeft.Get(entity);
            ref var roomPokerId = ref _roomPokerId.Get(entity);

            var playerLeft = roomPokerPlayerLeft.Player;

            roomPokerPlayers.Players.Remove(playerLeft);
            
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerLeft);
            ref var playerId = ref _playerId.Get(playerLeft);

            playerRoomPoker.RoomIds.Remove(roomPokerId.Value);

            if (playerRoomPoker.RoomIds.Count == 0)
            {
                _playerRoomPoker.Remove(playerLeft);
            }

            foreach (var pair in roomPokerPlayers.Players)
            {
                var player = pair.Key;
                
                _playerRoomRemoteLeftSend.Set(player, new PlayerRoomRemoteLeftSend
                {
                    RoomId = roomPokerId.Value,
                    IsAll = false,
                    PlayerId = playerId.Id,
                });
            }

            _roomPokerPlayerLeft.Remove(entity);

            if (roomPokerPlayers.Players.Count != 0)
            {
                continue;
            }
            
            _roomPokerStorage.Remove(roomPokerId.Value);
   
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}