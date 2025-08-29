using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

public class AwayPlayerAddRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerAwayAdd> _playerAwayAdd;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<AwayPlayerAddRequestDataframe>(Handler);
    }

    private void Handler(AwayPlayerAddRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var player))
        {
            return;
        }

        if (!_playerRoomPoker.Has(player))
        {
            return;
        }

        Console.WriteLine("Adding player away");
        _playerAwayAdd.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<AwayPlayerAddRequestDataframe>(Handler);
    }
}