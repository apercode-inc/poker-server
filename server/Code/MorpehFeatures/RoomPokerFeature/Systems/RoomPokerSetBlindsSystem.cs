using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetBlindsSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;

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
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            var markedPlayers = roomPokerPlayers.MarkedPlayersBySeat;
            var playersCount = roomPokerPlayers.MarkedPlayersBySeat.Count;

            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            var small = roomPokerStats.BigBet / 2;
            var big = roomPokerStats.BigBet;

            if (markedPlayers.TryGetValueByMarked(PokerPlayerMarkerType.ActivePlayer, out var nextPlayerByMarked))
            {
                if (playersCount > 2)
                {
                    var smallBlindPlayer = nextPlayerByMarked.Value;

                    _currencyPlayerService.TrySetBet(roomEntity, smallBlindPlayer, small);

                    markedPlayers.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out nextPlayerByMarked);

                    var bigBlindPlayer = nextPlayerByMarked.Value;

                    _currencyPlayerService.TrySetBet(roomEntity, bigBlindPlayer, big);

                    markedPlayers.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out nextPlayerByMarked);
                }
                else if (playersCount == 2)
                {
                    var smallBlindPlayer = nextPlayerByMarked.Value;
                    
                    _currencyPlayerService.TrySetBet(roomEntity, smallBlindPlayer, big);
                    
                    markedPlayers.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out nextPlayerByMarked);
                }
                
                _playerSetPokerTurn.Set(nextPlayerByMarked.Value);
            }

            _roomPokerSetBlinds.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}