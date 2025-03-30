using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerPokerCheck> _playerPokerCheck;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerMoveCompleteFlag> _playerMoveCompleteFlag;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerTransferMove> _roomPokerTransferMove;
    
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
            ref var playerSeat = ref _playerSeat.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            _playerPokerCheck.Remove(playerEntity);

            if (playerSeat.SeatIndex != roomPokerPlayers.MoverSeatPointer)
            {
                continue;
            }
            
            _roomPokerTransferMove.Set(roomEntity);
            _playerMoveCompleteFlag.Set(playerEntity);

        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}