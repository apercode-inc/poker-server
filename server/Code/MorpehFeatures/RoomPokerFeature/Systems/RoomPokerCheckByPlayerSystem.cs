using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerPokerCheck> _playerPokerCheck;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;

    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    private Filter _filter;
    
    public World World { get; set; }
    
    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerPokerCheck>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            if (roomPokerPlayers.MarkedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer,
                    out var nextPlayerByMarked))
            {
                _playerSetPokerTurn.Set(nextPlayerByMarked.Value);
            }

            _playerTurnCompleteFlag.Set(playerEntity);
            _playerPokerCheck.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}