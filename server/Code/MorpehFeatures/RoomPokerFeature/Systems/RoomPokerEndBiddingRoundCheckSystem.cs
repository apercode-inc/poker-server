using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerEndBiddingRoundCheckSystem : ISystem
{
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerTurnCompleteFlag> _playerTurnCompleteFlag;
    [Injectable] private Stash<PlayerAllin> _playerAllin;
    [Injectable] private Stash<PlayerCards> _playerCards;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerSetCardsToTable> _roomPokerSetCardsToTable;
    [Injectable] private Stash<RoomPokerShowdownForcedAllPlayers> _roomPokerShowdownForcedAllPlayers;
    [Injectable] private Stash<RoomPokerShowdownForcedAllPlayersDone> _roomPokerShowdownForcedAllPlayersDone;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerSetPokerTurn>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.TotalPlayersCount == 1)
            {
                continue;
            }
            
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            var isContinueBiddingRound = false;
            var allInCount = 0;

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (!playerBySeat.IsOccupied)
                {
                    continue;
                }
                
                var otherPlayer = playerBySeat.Player;

                if (playerEntity != otherPlayer && _playerAllin.Has(otherPlayer))
                {
                    allInCount++;
                    continue;
                }
                
                ref var playerCards = ref _playerCards.Get(otherPlayer);

                if (_playerAllin.Has(otherPlayer) || playerCards.CardsState == CardsState.Empty)
                {
                    continue;
                }
                
                if (!_playerTurnCompleteFlag.Has(otherPlayer))
                {
                    //Logger.LogWarning("isContinueBiddingRound = true [1]");
                    isContinueBiddingRound = true;
                }

                ref var otherPlayerPokerCurrentBet = ref _playerPokerCurrentBet.Get(otherPlayer);

                if (otherPlayerPokerCurrentBet.Value == roomPokerMaxBet.Value)
                {
                    continue;
                }
                //Logger.LogWarning("isContinueBiddingRound = true [2]");
                isContinueBiddingRound = true;
            }
            
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            var isCalled = playerPokerCurrentBet.Value >= roomPokerMaxBet.Value;

            if (isCalled && allInCount >= roomPokerPlayers.TotalPlayersCount - 1)
            {
                if (!_roomPokerShowdownForcedAllPlayersDone.Has(roomEntity))
                {
                    _roomPokerShowdownForcedAllPlayers.Set(roomEntity);
                }
                //Logger.LogWarning("isContinueBiddingRound = false [1]");
                isContinueBiddingRound = false;
            }

            if (isContinueBiddingRound)
            {
                continue;
            }

            roomPokerMaxBet.Value = 0;

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (!playerBySeat.IsOccupied)
                {
                    continue;
                }

                var player = playerBySeat.Player;
                ref var otherPlayerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
                otherPlayerPokerCurrentBet.Value = 0;
            }
            
            _playerSetPokerTurn.Remove(playerEntity);
            _roomPokerSetCardsToTable.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}