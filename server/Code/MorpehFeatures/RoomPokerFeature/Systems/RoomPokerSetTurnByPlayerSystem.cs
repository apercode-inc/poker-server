using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
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
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private NetFrameServer _server;

    private List<long> _raiseBets;
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _raiseBets = new List<long>();
        
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
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);
            
            var roomEntity = playerRoomPoker.RoomEntity;
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

            var requiredBet = roomPokerMaxBet.Value - playerPokerCurrentBet.Value; //250 - 50 = 200 --- надо доложить
            var remainderAfterCall = playerPokerContribution.Value - requiredBet; //1000 - 100 = 900 --- остаток вклада после ставки

            _raiseBets.Clear();
            
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
                requiredBet -= playerPokerContribution.Value; //250 - 100 = 150 ---- сколько надо доложить при ALL IN 
            }

            var raiseBet = requiredBet + roomPokerStats.BigBet;
            _raiseBets.Add(raiseBet);
            while (playerPokerContribution.Value > raiseBet)
            {
                raiseBet += roomPokerStats.BigBet;
                _raiseBets.Add(raiseBet);
            }
            _raiseBets.Add(playerPokerContribution.Value);

            var dataframe = new RoomPokerPlayerTurnRequestDataframe
            {
                TurnType = turnType,
                RequiredBet = requiredBet,
                RaiseBets = _raiseBets,
            };
            _server.Send(ref dataframe, playerEntity);
            
            _playerSetPokerTurn.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
        _raiseBets = null;
    }
}