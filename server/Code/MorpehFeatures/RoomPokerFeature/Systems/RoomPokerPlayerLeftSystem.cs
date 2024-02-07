using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerLeftSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayerLeft> _roomPokerPlayerLeft;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    [Injectable] private RoomPokerService _roomPokerService; 

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
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayerLeft = ref _roomPokerPlayerLeft.Get(roomEntity);
            var playerLeft = roomPokerPlayerLeft.Player;
            
            _roomPokerService.RemovePlayerFromRoom(roomEntity, playerLeft);

            _roomPokerPlayerLeft.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}