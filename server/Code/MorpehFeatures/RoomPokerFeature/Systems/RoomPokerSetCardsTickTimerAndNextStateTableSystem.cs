using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetCardsTickTimerAndNextStateTableSystem : ISystem
{
    [Injectable] private Stash<RoomPokerSetCardsTickTimer> _roomPokerSetCardsTickTimer;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerSetCardsTickTimer>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerSetCardsTickTimer = ref _roomPokerSetCardsTickTimer.Get(roomEntity);

            roomPokerSetCardsTickTimer.Value -= deltaTime;

            if (roomPokerSetCardsTickTimer.Value > 0)
            {
                continue;
            }

            _roomPokerSetCardsTickTimer.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (!roomPokerPlayers.MarkedPlayersBySeat.TryGetValueByMarked(PokerPlayerMarkerType.NextRoundActivePlayer,
                    out var markedPlayer))
            {
                continue;
            }

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (!playerBySeat.IsOccupied)
                {
                    continue;
                }
                _playerTurnCompleteFlag.Remove(playerBySeat.Player);
            }
            
            var activePlayer = markedPlayer.Value;
                
            roomPokerPlayers.MarkedPlayersBySeat.ResetMarkers(PokerPlayerMarkerType.ActivePlayer);
            roomPokerPlayers.MarkedPlayersBySeat.SetMarker(activePlayer, PokerPlayerMarkerType.ActivePlayer);
            
            _playerSetPokerTurn.Set(activePlayer);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}