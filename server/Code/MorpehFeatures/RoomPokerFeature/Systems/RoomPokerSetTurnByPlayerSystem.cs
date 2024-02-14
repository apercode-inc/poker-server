using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetTurnByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerSetPokerTurn>()
            .With<PlayerRoomPoker>()
            .Build();
    }
    
    // У игрока должен быть компонент PlayerCurrentBet
    // У стола должен быть компонент RoomPokerMaxBet
    // Call = RoomPokerMaxBet - PlayerCurrentBet,
    // если call положителен то значит состояние OnlyCallOrRaise
    // 
    
    //Player1 - 50
    //Player2 - 50
    //Player3 - 50
    //В Call показывать сколько ещё доложить надо
    //

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;

            //TODO Надо определить возможен ли чек, сколько колл, сколько можно рейз или это вообще только алл-ин !!!
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