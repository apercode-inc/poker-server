using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameCheckStartSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerGameStartTimer> _roomPokerGameStartTimer;

    [Injectable] private Stash<PlayerAway> _playerAway;
    
    [Injectable] private ConfigsService _configsService;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .Without<RoomPokerGameStartTimer>()
            .Without<RoomPokerActive>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            var activePlayersCount = 0;

            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }

                if (_playerAway.Has(player))
                {
                    continue;
                }

                activePlayersCount++;
            }

            if (activePlayersCount < 2)
            {
                continue;
            }

            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);

            _roomPokerGameStartTimer.Set(roomEntity, new RoomPokerGameStartTimer
            {
                Timer = 0,
                TargetTime = config.StartGameTime,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}