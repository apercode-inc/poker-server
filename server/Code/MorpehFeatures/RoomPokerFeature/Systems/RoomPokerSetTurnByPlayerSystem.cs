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
    
    //Player1 - 50
    //Player2 - 50
    //Player3 - 50
    //В Call показывать сколько ещё доложить надо
    //

    // Если call ноль или меньше то, CheckPossible
    // Если call положителен и remainderAfterCall положителен и не ноль, то OnlyCallOrRaise
    // Если call положителен и remainderAfterCall отрицателен и ноль, то OnlyAllIn

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);
            
            var roomEntity = playerRoomPoker.RoomEntity;
            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            var call = roomPokerMaxBet.Value - playerPokerCurrentBet.Value; //250 - 50 = 100 --- надо доложить
            var remainderAfterCall = playerPokerContribution.Value - call; //1000 - 100 = 900 --- Проверка можем ли доложить


            PokerPlayerTurnType turnType = default;
            if (call > 0 && remainderAfterCall >= 0)
            {
                turnType = PokerPlayerTurnType.OnlyCallOrRaise;
            }
            else if (remainderAfterCall >= 0)
            {
            }

            var dataframe = new RoomPokerPlayerTurnRequestDataframe
            {
                TurnType = PokerPlayerTurnType.OnlyCallOrRaise,
            };
            _server.Send(ref dataframe, playerEntity);
            
            _playerSetPokerTurn.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}