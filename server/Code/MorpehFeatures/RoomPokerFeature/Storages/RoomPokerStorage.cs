using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Storages;

public class RoomPokerStorage : IInitializer
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerData;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<Destroy> _destroy;

    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerRoomCreateSend> _playerRoomCreateSend;

    [Injectable] private RoomPokerSeatsFactory _pokerSeatsFactory;
    
    private Dictionary<int, Entity> _rooms;
    private int _idCounter;
    private Random _random;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _rooms = new Dictionary<int, Entity>();
        _random = new Random();
        
        _filter = World.Filter
            .With<RoomPokerId>()
            .Build();
    }

    public void Add(Entity createdPlayer, byte maxPlayers, CurrencyType currencyType, ulong contribution, ulong bigBet)
    {
        if (_playerRoomPoker.Has(createdPlayer))
        {
            Debug.LogError($"[RoomPokerStorageSystem.Add] the player is already in the room");
            return;
        }
        
        var newEntity = World.CreateEntity();
        var seat = (byte) _random.Next(0, maxPlayers);
        
        _roomPokerId.Set(newEntity, new RoomPokerId
        {
            Value = _idCounter,
        });
        _roomPokerData.Set(newEntity, new RoomPokerStats
        {
            MaxPlayers = maxPlayers,
            CurrencyType = currencyType,
            Contribution = contribution,
            BigBet = bigBet,
        });
        
        var markedPlayersBySeat = _pokerSeatsFactory
            .Create(maxPlayers, seat, createdPlayer);

        _roomPokerPlayers.Set(newEntity, new RoomPokerPlayers
        {
            MarkedPlayersBySeat = markedPlayersBySeat
        });

        _playerRoomPoker.Set(createdPlayer, new PlayerRoomPoker
        {
            RoomId = _idCounter,
        });
        
        _playerRoomCreateSend.Set(createdPlayer, new PlayerRoomCreateSend
        {
            RoomId = _idCounter,
            MaxPlayers = maxPlayers,
            Seat = seat,
        });

        _rooms.Add(_idCounter, newEntity);

        _idCounter++;
    }

    public bool TryGetById(int id, out Entity roomEntity)
    {
        if (_rooms.TryGetValue(id, out var value))
        {
            roomEntity = value;
            return true;
        }

        roomEntity = null;
        return false;
    }

    public void Remove(int id)
    {
        _rooms.Remove(id);
        
        foreach (var entity in _filter)
        {
            ref var roomPokerId = ref _roomPokerId.Get(entity);

            if (roomPokerId.Value == id)
            {
                _destroy.Set(entity);
                break;
            }
        }
    }

    public void Dispose()
    {
        _rooms = null;
        _filter = null;
        _random = null;
    }
}