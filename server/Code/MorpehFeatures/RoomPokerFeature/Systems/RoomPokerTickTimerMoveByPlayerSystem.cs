using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerTickTimerMoveByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerMoveTimer> _playerMoveTimer;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerMoveTimerReset> _playerMoveTimerReset;
    [Injectable] private Stash<PlayerPokerCheck> _playerPokerCheck;
    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerAway> _playerAway;
    
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerRoomPoker>()
            .With<PlayerMoveTimer>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerMoveTimer = ref _playerMoveTimer.Get(playerEntity);

            playerMoveTimer.TimeCurrent += deltaTime;

            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;

            if (playerMoveTimer.TimeCurrent < playerMoveTimer.TimeMax && !_playerAway.Has(playerEntity))
            {
                continue;
            }
            
            ref var playerPokerCurrentBet = ref _playerPokerCurrentBet.Get(playerEntity);

            ref var roomPokerMaxBet = ref _roomPokerMaxBet.Get(roomEntity);

            if (roomPokerMaxBet.Value - playerPokerCurrentBet.Value > 0)
            {
                _playerDropCards.Set(playerEntity);
            }
            else
            {
                _playerPokerCheck.Set(playerEntity);
            }

            var dataframe = new RoomPokerPlayerActiveHudPanelCloseDataframe();
            _server.Send(ref dataframe, playerEntity);

            _playerMoveTimerReset.Set(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}