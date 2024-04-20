using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowOrHideCardsActivateSystem : ISystem
{
    [Injectable] private Stash<RoomPokerShowOrHideCards> _roomPokerShowOrHideCards;
    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;

    [Injectable] private Stash<PlayerShowOrHideTimer> _playerShowOrHideTimer;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private ConfigsService _configsService;
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerStats>()
            .With<RoomPokerShowOrHideCards>()
            .With<RoomPokerShowOrHideCardsActivate>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerShowOrHideCardsActivate.Remove(roomEntity);
            
            ref var roomPokerShowOrHideCards = ref _roomPokerShowOrHideCards.Get(roomEntity);

            if (roomPokerShowOrHideCards.Players.TryDequeue(out var playerEntity))
            {
                if (playerEntity.IsNullOrDisposed())
                {
                    _roomPokerShowOrHideCardsActivate.Set(roomEntity);
                }
                else
                {
                    ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);

                    _playerShowOrHideTimer.Set(playerEntity, new PlayerShowOrHideTimer
                    {
                        TimeCurrent = 0,
                        TimeMax = roomPokerStats.TurnTime,
                    });
                    
                    var dataframe = new RoomPokerPlayerTurnRequestDataframe
                    {
                        TurnType = PokerPlayerTurnType.Showdown
                    };
                    _server.Send(ref dataframe, playerEntity);

                    ref var playerId = ref _playerId.Get(playerEntity);
                    
                    var timeDataframe = new RoomPokerSetTimerTurnDataframe
                    {
                        PlayerId = playerId.Id,
                        Time = roomPokerStats.TurnTime,
                    };
                    _server.SendInRoom(ref timeDataframe, roomEntity);
                }
            }
            else
            {
                var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
                
                _roomPokerCleanupTimer.Set(roomEntity,new RoomPokerCleanupTimer
                {
                    Value = config.DelayCleanup,
                });
                _roomPokerShowOrHideCards.Remove(roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}