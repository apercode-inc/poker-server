using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCreateRequestSyncSystem : IInitializer
{
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorageSystem _playerStorage;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerCreateRequestDataframe>(DataframeHandler);
    }

    private void DataframeHandler(RoomPokerCreateRequestDataframe dataframe, int id)
    {
        if (_playerStorage.TryGetPlayerById(id, out var player))
        {
            _roomPokerStorage.Add(player, dataframe.MaxPlayers, dataframe.SmallBet, dataframe.BigBet);
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerCreateRequestDataframe>(DataframeHandler);
    }
}