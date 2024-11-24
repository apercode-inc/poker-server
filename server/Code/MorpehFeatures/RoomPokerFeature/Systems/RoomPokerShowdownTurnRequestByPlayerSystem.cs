using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownTurnRequestByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerPokerShowdownTurnRequest> _playerPokerShowdownTurnRequest;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerTurnShowdownTimer> _playerTurnShowdownTimer;

    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerPokerShowdownTurnRequest>()
            .With<PlayerRoomPoker>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            var dataframe = new RoomPokerPlayerTurnRequestDataframe
            {
                TurnType = PokerPlayerTurnType.Showdown,
            };
            _server.Send(ref dataframe, playerEntity);

            ref var playerId = ref _playerId.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);

            var roomEntity = playerRoomPoker.RoomEntity;
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
                
            var timeDataframe = new RoomPokerSetTimerTurnDataframe
            {
                PlayerId = playerId.Id,
                Time = roomPokerStats.TurnTime,
            };
            _server.SendInRoom(ref timeDataframe, roomEntity);
            
            _playerTurnShowdownTimer.Set(playerEntity, new PlayerTurnShowdownTimer
            {
                TimeCurrent = 0,
                TimeMax = roomPokerStats.TurnTime,
            });
            
            _playerPokerShowdownTurnRequest.Remove(playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}