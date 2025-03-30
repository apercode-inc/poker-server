using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetBlindsSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerSeat> _playerSeat;

    [Injectable] private CurrencyPlayerService _currencyPlayerService;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerStats>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerSetBlinds>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            var small = roomPokerStats.BigBet / 2;
            var big = roomPokerStats.BigBet;

            var startSeatIndex = roomPokerPlayers.DealerSeatPointer;
            var playerCount = roomPokerPlayers.PlayersBySeat.Length;

            _roomPokerSetBlinds.Remove(roomEntity);
            
            if (roomPokerPlayers.TotalPlayersCount <= 1)
            {
                continue;
            }

            for (int i = 1, playerCounter = 0; playerCounter < 3; i++)
            {
                var nextSeatIndex = (startSeatIndex + i) % playerCount;
                var nextPlayerEntity = roomPokerPlayers.PlayersBySeat[nextSeatIndex];

                if (nextPlayerEntity.IsNullOrDisposed() || _playerAway.Has(nextPlayerEntity))
                {
                    continue;
                }

                if (playerCounter < 2)
                {
                    _currencyPlayerService.TrySetBet(roomEntity, nextPlayerEntity, playerCounter == 0 ? small : big);
                }
                else
                {
                    ref var playerSeat = ref _playerSeat.Get(nextPlayerEntity);
                    roomPokerPlayers.MoverSeatPointer = playerSeat.SeatIndex;
                }
                
                playerCounter++;
            }

            roomPokerMaxBet.Value = roomPokerStats.BigBet;
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}