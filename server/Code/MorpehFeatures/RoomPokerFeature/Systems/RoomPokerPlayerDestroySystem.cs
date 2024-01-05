using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerDestroySystem : ILateSystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private RoomPokerStorageSystem _roomPokerStorage;
    
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
        foreach (var entity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(entity);

            foreach (var roomId in playerRoomPoker.RoomIds)
            {
                if (!_roomPokerStorage.TryGetById(roomId, out var roomEntity))
                {
                    continue;
                }
                
                ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

                roomPokerPlayers.Players.Remove(entity);
                
                if (roomPokerPlayers.Players.Count != 0)
                {
                    continue;
                }

                _roomPokerStorage.Remove(roomId);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}