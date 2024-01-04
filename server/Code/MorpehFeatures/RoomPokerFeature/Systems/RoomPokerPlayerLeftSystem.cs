using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerLeftSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayerLeft> _roomPokerPlayerLeft;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;

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

            ref var roomPokerPlayerJoin = ref _roomPokerPlayerLeft.Get(entity);
            
            roomPokerPlayers.Players.Remove(roomPokerPlayerJoin.Player);

            _roomPokerPlayerLeft.Remove(entity);
            
            if (roomPokerPlayers.Players.Count == 0)
            {
                ref var roomPokerId = ref _roomPokerId.Get(entity);
                
                _roomPokerStorage.Remove(roomPokerId.Value);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}