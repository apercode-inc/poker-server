using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerStorageSystem : IInitializer
{
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<Destroy> _destroy;

    private Filter _filter;

    private Dictionary<int, Entity> _players;

    public World World { get; set; }

    public void OnAwake()
    {
        _players = new Dictionary<int, Entity>();
        
        _filter = World.Filter
            .With<PlayerId>()
            .Build();
    }

    public void Add(int id)
    {
        var newEntity = World.CreateEntity();
        _playerId.Set(newEntity, new PlayerId
        {
            Id = id,
        });
        _players.Add(id, newEntity);
        
        //Подгрузка из бд и навешивание PlayerAuthData
    }

    public void Remove(int id)
    {
        _players.Remove(id);
        
        foreach (var entity in _filter)
        {
            ref var playerId = ref _playerId.Get(entity);

            if (playerId.Id == id)
            {
                _destroy.Set(entity);
                break;
            }
        }
    }

    public bool TryGetPlayerById(int id, out Entity player)
    {
        if (_players.TryGetValue(id, out var value))
        {
            player = value;
            return true;
        }

        player = null;
        return false;
    }

    public void Dispose()
    {
        _filter = null;
    }
}