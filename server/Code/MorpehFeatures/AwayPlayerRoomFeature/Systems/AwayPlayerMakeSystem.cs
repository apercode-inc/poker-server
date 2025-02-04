using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerMakeSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerAwayMake> _playerAwayMake;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private RoomPokerService _roomPokerService;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAwayMake>()
            .With<PlayerRoomPoker>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerAway.Set(playerEntity);
            _playerAwayMake.Remove(playerEntity);

            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(playerRoomPoker.RoomEntity);
            
            roomPokerPlayers.AwayPlayers.Add(playerEntity);
            
            _roomPokerService.RemoveFromMarkedPlayers(playerRoomPoker.RoomEntity, playerEntity, 
                roomPokerPlayers.MarkedPlayersBySeat);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}