using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerLeftRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<RoomPokerPlayerLeft> _roomPokerPlayerLeft;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorageSystem _playerStorage;
    [Injectable] private RoomPokerStorageSystem _roomPokerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerLeftRequestDataframe>(DataframeHandler);
    }

    private void DataframeHandler(RoomPokerLeftRequestDataframe dataframe, int id)
    {
        if (_playerStorage.TryGetPlayerById(id, out var player))
        {
            if (_roomPokerStorage.TryGetById(dataframe.RoomId, out var room))
            {
                _roomPokerPlayerLeft.Set(room, new RoomPokerPlayerLeft
                {
                    Player = player,
                });
            }
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerLeftRequestDataframe>(DataframeHandler);
    }
}