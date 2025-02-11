using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.TopUpFeature.Components;
using server.Code.MorpehFeatures.TopUpFeature.Dataframes;

namespace server.Code.MorpehFeatures.TopUpFeature.Systems;

public class TopUpConfirmRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerTopUp> _playerTopUp;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<TopUpConfirmRequestDataframe>(Handler);
    }

    private void Handler(TopUpConfirmRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var playerEntity))
        {
            return;
        }
        
        _playerTopUp.Set(playerEntity, new PlayerTopUp
        {
            Value = dataframe.Value,
        });
    }

    public void Dispose()
    {
        _server.Unsubscribe<TopUpConfirmRequestDataframe>(Handler);
    }
}