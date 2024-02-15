using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetBetByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerSetBet> _roomPokerSetBet;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    
    public World World { get; set; }

    private Filter _filter;
    
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
            ref var roomPokerSetBet = ref _roomPokerSetBet.Get(playerEntity);
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

            _roomPokerSetBet.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}