using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetBetByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerSetBet> _playerSetBet;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerMoveCompleteFlag> _playerMoveCompleteFlag;
    
    [Injectable] private Stash<RoomPokerTransferMove> _roomPokerTransferMove;
    
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    
    private Filter _filter;
    
    public World World { get; set; }
    
    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerSetBet>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var roomPokerSetBet = ref _playerSetBet.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var bet = roomPokerSetBet.Bet;

            var roomEntity = playerRoomPoker.RoomEntity;

            _playerSetBet.Remove(playerEntity);

            if (!_currencyPlayerService.TrySetBet(roomEntity, playerEntity, bet))
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