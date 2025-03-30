using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Move;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerHudShowCardsRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerShowdownForced> _playerShowdownForced;
    [Injectable] private Stash<PlayerMoveShowdownResetTimer> _playerMoveShowdownResetTimer;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerHudShowCardsRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerHudShowCardsRequestDataframe dataframe, int playerId)
    {
        if (!_playerStorage.TryGetPlayerById(playerId, out var playerEntity))
        {
            return;
        }
        
        _playerShowdownForced.Set(playerEntity);
        _playerMoveShowdownResetTimer.Set(playerEntity);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerHudShowCardsRequestDataframe>(Handler);
    }
}