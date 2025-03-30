using System.Text;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerCheckTestSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    private Filter _filter;
    private float _timer;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void Dispose()
    {
        _filter = null;
    }

    public void OnUpdate(float deltaTime)
    {
        _timer += deltaTime;

        if (_timer < 2.0f)
        {
            return;
        }

        _timer = 0;
        
        foreach (var entity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);

            var sb = new StringBuilder();

            for (var index = 0; index < roomPokerPlayers.PlayersBySeat.Length; index++)
            {
                var playerBySeat = roomPokerPlayers.PlayersBySeat[index];
                var x = playerBySeat.Player.IsNullOrDisposed() ? 0 : 1;
                //var y = playerBySeat.IsOccupied ? 1 : 0;
                sb.Append($" p:{x} |");
                //sb.Append($" p:{x} s:{y} |");
            }

            //Logger.LogWarning($"total:{roomPokerPlayers.TotalPlayersCount} ---> {sb}");
        }
    }
}