using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetTurnByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerTurnTimer> _playerTurnTimer;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerPokerCurrentBet>()
            .With<PlayerRoomPoker>()
            .With<PlayerPokerContribution>()
            .With<PlayerSetPokerTurn>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerSetPokerTurn.Remove(playerEntity);
            
            ref var playerCards = ref _playerCards.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;

            if (playerCards.CardsState == CardsState.Empty)
            {
                ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

                if (roomPokerPlayers.MarkedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer,
                        out var nextPlayerByMarked))
                {
                    _playerSetPokerTurn.Set(nextPlayerByMarked.Value);
                }
                
                continue;
            }
            
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);
            ref var playerId = ref _playerId.Get(playerEntity);
            
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

            if (roomPokerMaxBet.Value < roomPokerStats.BigBet)
            {
                roomPokerMaxBet.Value = roomPokerStats.BigBet;
            }

            var requiredBet = roomPokerMaxBet.Value - playerPokerCurrentBet.Value;
            var remainderAfterCall = playerPokerContribution.Value - requiredBet;

            PokerPlayerTurnType turnType;
            
            if (requiredBet <= 0)
            {
                turnType = PokerPlayerTurnType.CheckPossible;
            }
            else if (remainderAfterCall > 0)
            {
                turnType = PokerPlayerTurnType.OnlyCallOrRaise;
            }
            else
            {
                turnType = PokerPlayerTurnType.OnlyAllIn;
                requiredBet -= playerPokerContribution.Value;
            }

            var raiseBets = new List<long>();
            
            var raiseBet = requiredBet + roomPokerStats.BigBet;
            raiseBets.Add(raiseBet);
            while (playerPokerContribution.Value > raiseBet)
            {
                raiseBet += roomPokerStats.BigBet;
                raiseBets.Add(raiseBet);
            }
            raiseBets.Add(playerPokerContribution.Value);

            var dataframe = new RoomPokerPlayerTurnRequestDataframe
            {
                TurnType = turnType,
                RequiredBet = requiredBet,
                RaiseBets = raiseBets,
            };
            _server.Send(ref dataframe, playerEntity);

            var timeDataframe = new RoomPokerSetTurnTimerDataframe
            {
                PlayerId = playerId.Id,
                Time = roomPokerStats.TurnTime,
            };
            _server.SendInRoom(ref timeDataframe, roomEntity);
            
            _playerTurnTimer.Set(playerEntity, new PlayerTurnTimer
            {
                Timer = 0,
                TurnTime = roomPokerStats.TurnTime,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}