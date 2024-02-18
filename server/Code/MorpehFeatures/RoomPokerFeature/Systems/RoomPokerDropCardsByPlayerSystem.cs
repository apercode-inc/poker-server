using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDropCardsByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerDropCards> _playerDropCards;

    [Injectable] private RoomPokerService _roomPokerService;
    
    private Filter _filter;
    
    public World World { get; set; }
    
    
    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerDropCards>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            _roomPokerService.DropCards(roomEntity, playerEntity);
            
            _playerDropCards.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}