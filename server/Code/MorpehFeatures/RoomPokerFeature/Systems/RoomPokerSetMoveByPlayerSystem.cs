using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetMoveByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerMove> _playerSetPokerMove;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerMoveTimer> _playerMoveTimer;
    [Injectable] private Stash<PlayerAllin> _playerAllin;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;
    [Injectable] private Stash<RoomPokerTransferMove> _roomPokerTransferMove;
    
    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerPokerCurrentBet>()
            .With<PlayerRoomPoker>()
            .With<PlayerPokerContribution>()
            .With<PlayerSetPokerMove>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerSetPokerMove.Remove(playerEntity);
            
            ref var playerCards = ref _playerCards.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.TotalPlayersCount == 1)
            {
                continue;
            }

            if (_roomPokerOnePlayerRoundGame.Has(roomEntity))
            {
                continue;
            }

            if (playerCards.CardsState == CardsState.Empty || _playerAllin.Has(playerEntity))
            {
                _roomPokerTransferMove.Set(roomEntity);
                continue;
            }
            
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            
            _playerMoveTimer.Set(playerEntity, new PlayerMoveTimer
            {
                TimeCurrent = 0,
                TimeMax = roomPokerStats.MoveTime,
            });

            if (_playerAway.Has(playerEntity))
            {
                continue;
            }

            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);
            ref var playerId = ref _playerId.Get(playerEntity);
            
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            var requiredBet = roomPokerMaxBet.Value - playerPokerCurrentBet.Value;
            var remainderAfterCall = playerPokerContribution.Value - requiredBet;

            PokerPlayerMoveType moveType;
            
            if (requiredBet <= 0)
            {
                moveType = PokerPlayerMoveType.CheckPossible;
            }
            else if (remainderAfterCall > 0)
            {
                moveType = PokerPlayerMoveType.OnlyCallOrRaise;
            }
            else
            {
                moveType = PokerPlayerMoveType.OnlyAllIn;
                requiredBet -= playerPokerContribution.Value;
            }
           
            var dataframe = new RoomPokerPlayerMoveRequestDataframe
            {
                MoveType = moveType,
                RequiredBet = requiredBet,
            };
            _server.Send(ref dataframe, playerEntity);

            var timeDataframe = new RoomPokerSetTimerMoveDataframe
            {
                PlayerId = playerId.Id,
                Time = roomPokerStats.MoveTime,
            };
            _server.SendInRoom(ref timeDataframe, roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}