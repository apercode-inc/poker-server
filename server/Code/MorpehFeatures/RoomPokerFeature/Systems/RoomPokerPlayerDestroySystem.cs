using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerDestroySystem : ILateSystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private RoomPokerStorage _roomPokerStorage;
    [Injectable] private RoomPokerService _roomPokerService; 
    
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
            var roomEntity = playerRoomPoker.RoomEntity;

            _roomPokerService.RemovePlayerFromRoom(roomEntity, playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}