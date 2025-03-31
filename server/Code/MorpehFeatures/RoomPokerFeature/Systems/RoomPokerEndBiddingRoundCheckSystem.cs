using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerEndBiddingRoundCheckSystem : ISystem
{
    [Injectable] private Stash<PlayerSetPokerMove> _playerSetPokerMove;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerMoveCompleteFlag> _playerMoveCompleteFlag;
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
            .With<PlayerSetPokerMove>()
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
            var withCardsCount = 0;

            foreach (var otherPlayer in roomPokerPlayers.PlayersBySeat)
            {
                if (otherPlayer.IsNullOrDisposed())
                {
                    continue;
                }
                
                ref var playerCards = ref _playerCards.Get(otherPlayer);
                
                if (playerCards.CardsState != CardsState.Empty)
                {
                    withCardsCount++;
                }

                if (playerEntity != otherPlayer && _playerAllin.Has(otherPlayer))
                {
                    allInCount++;
                    continue;
                }

                if (_playerAllin.Has(otherPlayer) || playerCards.CardsState == CardsState.Empty)
                {
                    continue;
                }

                if (!_playerMoveCompleteFlag.Has(otherPlayer))
                {
                    isContinueBiddingRound = true;
                }

                ref var otherPlayerPokerCurrentBet = ref _playerPokerCurrentBet.Get(otherPlayer);

                if (otherPlayerPokerCurrentBet.Value == roomPokerMaxBet.Value)
                {
                    continue;
                }
                isContinueBiddingRound = true;
            }
            
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            var isCalled = playerPokerCurrentBet.Value >= roomPokerMaxBet.Value;

            var withCardsCountWithoutOne = withCardsCount - 1;
            
            if (isCalled && withCardsCountWithoutOne != 0 && allInCount >= withCardsCountWithoutOne)
            {
                if (!_roomPokerShowdownForcedAllPlayersDone.Has(roomEntity))
                {
                    _roomPokerShowdownForcedAllPlayers.Set(roomEntity);
                }
                isContinueBiddingRound = false;
            }

            if (isContinueBiddingRound)
            {
                continue;
            }

            roomPokerMaxBet.Value = 0;

            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }
                
                ref var otherPlayerPokerCurrentBet = ref _playerPokerCurrentBet.Get(player);
                otherPlayerPokerCurrentBet.Value = 0;
            }
            
            _playerSetPokerMove.Remove(playerEntity);
            _roomPokerSetCardsToTable.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}