using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerAwayAdd> _playerAwayAdd;
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<AwayPlayerRequestDataframe>(Handler);
    }

    private void Handler(AwayPlayerRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var player))
        {
            return;
        }

        if (dataframe.IsAway)
        {
            _playerAwayAdd.Set(player);
        }
        else
        {
            _playerAwayRemove.Set(player);
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<AwayPlayerRequestDataframe>(Handler);
    }
}