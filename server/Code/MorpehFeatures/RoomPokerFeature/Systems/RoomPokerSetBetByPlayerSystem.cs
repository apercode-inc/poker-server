using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetBetByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerSetBet> _playerSetBet;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
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

            var roomEntity = playerRoomPoker.RoomEntity;

            if (_currencyPlayerService.TrySetBet(roomEntity, playerEntity, roomPokerSetBet.Bet))
            {
                ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
                var markedPlayers = roomPokerPlayers.MarkedPlayersBySeat;

                if (markedPlayers.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out var nextPlayerByMarked))
                {
                    _playerSetPokerTurn.Set(nextPlayerByMarked.Value);
                }
            }
            else
            {
                Logger.Error($"[RoomPokerSetBetByPlayerSystem.OnUpdate] error set bet");
            }
            
            _playerTurnCompleteFlag.Set(playerEntity);
            _playerSetBet.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}