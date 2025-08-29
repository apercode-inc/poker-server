using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerRemoveRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<AwayPlayerRemoveRequestDataframe>(Handler);
    }

    private void Handler(AwayPlayerRemoveRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var player))
        {
            return;
        }

        if (!_playerRoomPoker.Has(player))
        {
            return;
        }
        
        _playerAwayRemove.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<AwayPlayerRemoveRequestDataframe>(Handler);
    }
}