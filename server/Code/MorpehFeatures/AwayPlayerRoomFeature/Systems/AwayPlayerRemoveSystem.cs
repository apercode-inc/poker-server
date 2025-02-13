using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerRemoveSystem : ISystem
{
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAwayRemove>()
            .With<PlayerRoomPoker>()
            .With<PlayerSeat>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerSeat = ref _playerSeat.Get(playerEntity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(playerRoomPoker.RoomEntity);

            roomPokerPlayers.MarkedPlayersBySeat.Add(playerSeat.SeatIndex, playerEntity);

            _playerAway.Remove(playerEntity);
            _playerAwayRemove.Remove(playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}