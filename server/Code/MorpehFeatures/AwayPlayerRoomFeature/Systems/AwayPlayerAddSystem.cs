using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerAddSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerAwayAdd> _playerAwayAdd;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private RoomPokerService _roomPokerService;
    [Injectable] private ConfigsService _configsService;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerAwayAdd>()
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
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(playerRoomPoker.RoomEntity);
            
            roomPokerPlayers.AwayPlayers.Add(playerEntity);
            
            _roomPokerService.RemoveAwayPlayer(playerRoomPoker.RoomEntity, playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}