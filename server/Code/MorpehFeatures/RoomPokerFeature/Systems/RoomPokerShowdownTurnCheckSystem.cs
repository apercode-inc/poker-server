using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownTurnCheckSystem : ISystem
{
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerPokerShowdownTurnRequest> _playerPokerShowdownTurnRequest;

    [Injectable] private ConfigsService _configsService;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerShowdownChoiceCheck>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerShowdownChoiceCheck.Remove(roomEntity);
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            var isSkipCleanup = false;

            foreach (var playerBySeat in roomPokerPlayers.PlayersBySeat)
            {
                if (playerBySeat.Player.IsNullOrDisposed())
                {
                    continue;
                }
                
                var playerEntity = playerBySeat.Player;

                ref var playerCards = ref _playerCards.Get(playerEntity);

                if (playerCards.CardsState != CardsState.Close)
                {
                    continue;
                }
                
                isSkipCleanup = true;
                _playerPokerShowdownTurnRequest.Set(playerEntity);
                    
                break;
            }

            if (isSkipCleanup)
            {
                continue;
            }
            
            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
            _roomPokerCleanupTimer.Set(roomEntity, new RoomPokerCleanupTimer
            {
                Value = config.DelayCleanup,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}