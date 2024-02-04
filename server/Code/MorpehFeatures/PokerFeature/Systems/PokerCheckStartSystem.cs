using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerCheckStartSystem : ISystem
{
    private const float WAIT_TIME = 15.0f;
    
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PokerStartTimer> _pokerStartTimer;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .Without<PokerStartTimer>()
            .Without<PokerActive>()
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

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var dataframe = new PokerStartGameSetTimerDataframe
                {
                    WaitTime = (int) WAIT_TIME,
                };
                _server.SendInRoom(ref dataframe, roomEntity);
            }
            
            _pokerStartTimer.Set(roomEntity, new PokerStartTimer
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