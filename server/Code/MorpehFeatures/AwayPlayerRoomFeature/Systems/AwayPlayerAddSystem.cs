using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerAddSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerAwayAdd> _playerAwayAdd;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private RoomPokerService _roomPokerService;
    [Injectable] private ConfigsService _configsService;
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAwayAdd>()
            .With<PlayerId>()
            .With<PlayerRoomPoker>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerAwayAdd.Remove(playerEntity);

            if (_playerAway.Has(playerEntity))
            {
                continue;
            }
            
            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
            
            _playerAway.Set(playerEntity, new PlayerAway
            {
                Timer = config.AwayPlayerTime,
            });

            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerId = ref _playerId.Get(playerEntity);

            var roomEntity = playerRoomPoker.RoomEntity;

            var dataframe = new AwayPlayerTimerDataframe
            {
                PlayerId = playerId.Id,
                Time = config.AwayPlayerTime,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}