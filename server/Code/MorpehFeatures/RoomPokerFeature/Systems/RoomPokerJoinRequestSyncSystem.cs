using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerJoinRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<RoomPokerPlayerJoin> _roomPokerPlayerJoin;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorageSystem _playerStorage;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerJoinRequestDataframe>(DataframeHandler);
    }

    private void DataframeHandler(RoomPokerJoinRequestDataframe dataframe, int id)
    {
        if (_playerStorage.TryGetPlayerById(id, out var player))
        {
            if (_roomPokerStorage.TryGetById(dataframe.RoomId, out var room))
            {
                _roomPokerPlayerJoin.Set(room, new RoomPokerPlayerJoin
                {
                    Player = player,
                });
            }
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerJoinRequestDataframe>(DataframeHandler);
    }
}