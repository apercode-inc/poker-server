using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerJoinRequestSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomCreateSend> _playerRoomCreateSend;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;

    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private RoomPokerStorage _roomPokerStorage;

    private Random _random;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _random = new Random();
        _server.Subscribe<RoomPokerJoinRequestDataframe>(DataframeHandler);
    }

    private void DataframeHandler(RoomPokerJoinRequestDataframe dataframe, int id)
    {
        if (!_playerStorage.TryGetPlayerById(id, out var player))
        {
            return;
        }

        if (!_roomPokerStorage.TryGetById(dataframe.RoomId, out var roomEntity))
        {
            return;
        }

        ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        if (roomPokerPlayers.MarkedPlayersBySeat.Count >= roomPokerStats.MaxPlayers)
        {
            //todo
            Logger.Error("отправить игроку нотиф о том что он не может присоединится из за того что игроков максимум");
            return;
        }
        
        ref var playerCurrency = ref _playerCurrency.Get(player, out var currencyExist);

        if (!currencyExist)
        {
            return;
        }

        if (playerCurrency.CurrencyByType[roomPokerStats.CurrencyType] < roomPokerStats.Contribution)
        {
            //todo
            Logger.Error("отправить игроку нотиф о том что он не может присоединится из за того что ему не хватает на взнос");
            return;
        }

        var freeSeats = new FastList<byte>();

        for (byte index = 0; index < roomPokerStats.MaxPlayers; index++)
        {
            if (!roomPokerPlayers.MarkedPlayersBySeat.ContainsKey(index))
            {
                freeSeats.Add(index);
            }
        }
        
        var randomIndex = _random.Next(0, freeSeats.length);
        var seatIndex = freeSeats.data[randomIndex];

        roomPokerPlayers.MarkedPlayersBySeat.Add(seatIndex, player);

        _playerStorage.CreateForRoomAndSync(player, roomPokerStats.CurrencyType, roomPokerStats.Contribution, roomEntity, seatIndex);

        _playerRoomCreateSend.Set(player);
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerJoinRequestDataframe>(DataframeHandler);
        _random = null;
    }
}