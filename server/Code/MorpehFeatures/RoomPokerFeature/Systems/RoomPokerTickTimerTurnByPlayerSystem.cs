using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTickTimerTurnByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerTurnTimer> _playerTurnTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerSetPokerTurn> _playerSetPokerTurn;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerTurnTimerReset> _playerTurnTimerReset;
    [Injectable] private Stash<PlayerPokerCheck> _playerPokerCheck;
    
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerService _roomPokerService;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerRoomPoker>()
            .With<PlayerTurnTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerTurnTimer = ref _playerTurnTimer.Get(playerEntity);

            playerTurnTimer.Timer += deltaTime;

            if (playerTurnTimer.Timer < playerTurnTimer.TurnTime)
            {
                continue;
            }
            
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;

            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            if (roomPokerMaxBet.Value - playerPokerCurrentBet.Value > 0)
            {
                _roomPokerService.DropCards(roomEntity, playerEntity);
            }
            else
            {
                _playerPokerCheck.Set(playerEntity);
            }

            var dataframe = new RoomPokerPlayerActiveHudPanelCloseDataframe();
            _server.Send(ref dataframe, playerEntity);
            
            _playerTurnTimerReset.Set(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}