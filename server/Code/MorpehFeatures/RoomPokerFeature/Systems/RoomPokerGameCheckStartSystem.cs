using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameCheckStartSystem : ISystem
{
    private const float WAIT_TIME = 15.0f;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerGameStartTimer> _roomPokerGameStartTimer;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .Without<RoomPokerGameStartTimer>()
            .Without<RoomPokerActive>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.MarkedPlayersBySeat.Count < 2)
            {
                continue;
            }

            _roomPokerGameStartTimer.Set(roomEntity, new RoomPokerGameStartTimer
            {
                Timer = 0,
                TargetTime = WAIT_TIME,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}