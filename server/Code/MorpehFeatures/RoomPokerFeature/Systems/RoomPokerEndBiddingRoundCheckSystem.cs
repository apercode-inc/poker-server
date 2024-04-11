using Scellecs.Morpeh;
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
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            var isContinueBiddingRound = false;

            foreach (var markedPlayers in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayers.Value;
                
                ref var playerCards = ref _playerCards.Get(player);

                if (_playerAllin.Has(player) || playerCards.CardsState == CardsState.Empty)
                {
                    continue;
                }
                
                if (!_playerTurnCompleteFlag.Has(player))
                {
                    isContinueBiddingRound = true;
                    break;
                }

                ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);

                if (playerPokerCurrentBet.Value != roomPokerMaxBet.Value)
                {
                    isContinueBiddingRound = true;
                    break;
                }
            }

            if (isContinueBiddingRound)
            {
                continue;
            }

            roomPokerMaxBet.Value = 0;

            foreach (var markedPlayers in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayers.Value;
                ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
                playerPokerCurrentBet.Value = 0;
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