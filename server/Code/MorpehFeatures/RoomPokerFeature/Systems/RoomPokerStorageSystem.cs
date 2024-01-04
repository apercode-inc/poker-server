using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerStorageSystem : IInitializer
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerData;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    private Dictionary<int, Entity> _rooms;
    private int _idCounter;

    public World World { get; set; }

    public void OnAwake()
    {
        _rooms = new Dictionary<int, Entity>();
    }

    public void Add(Entity createdPlayer, byte maxPlayers, ulong smallBet, ulong bigBet)
    {
        var newEntity = World.CreateEntity();
        _roomPokerId.Set(newEntity, new RoomPokerId
        {
            Value = _idCounter,
        });
        _roomPokerData.Set(newEntity, new RoomPokerStats
        {
            MaxPlayers = maxPlayers,
            SmallBet = smallBet,
            BigBet = bigBet,
        });
        _roomPokerPlayers.Set(newEntity, new RoomPokerPlayers
        {
            Players = new List<Entity>{ createdPlayer }
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
    }

    public void Dispose()
    {
        _rooms = null;
    }
}