using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerDestroySystem : ILateSystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private Stash<PlayerRoomRemoteLeftSend> _playerRoomRemoteLeftSend;

    [Injectable] private RoomPokerStorage _roomPokerStorage;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerRoomPoker>()
            .With<Destroy>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerId = ref _playerId.Get(playerEntity);

            var roomId = playerRoomPoker.RoomId;


            if (!_roomPokerStorage.TryGetById(roomId, out var roomEntity))
            {
                continue;
            }

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            var isRemoved = roomPokerPlayers.MarkedPlayersBySeat.Remove(playerEntity);

            if (isRemoved)
            {
                foreach (var player in roomPokerPlayers.MarkedPlayersBySeat)
                {
                    _playerRoomRemoteLeftSend.Set(player.Value, new PlayerRoomRemoteLeftSend
                    {
                        RoomId = roomId,
                        PlayerId = playerId.Id,
                    });
                }
            }

            if (roomPokerPlayers.MarkedPlayersBySeat.Count != 0)
            {
                continue;
            }

            _roomPokerStorage.Remove(roomId);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}