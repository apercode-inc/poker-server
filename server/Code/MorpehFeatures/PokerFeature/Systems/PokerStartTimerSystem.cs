using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.PokerFeature.Dataframes.StartTimer;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerStartTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PokerStartTimer> _pokerStartTimer;
    [Injectable] private Stash<PokerInitialize> _pokerInitialize;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .With<PokerStartTimer>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.MarkedPlayersBySeat.Count < 2)
            {
                _pokerStartTimer.Remove(roomEntity);
                
                var dataframe = new PokerStopGameResetTimerDataframe();
                _server.SendInRoom(ref dataframe, roomEntity);
            }
            
            ref var pokerStartTimer = ref _pokerStartTimer.Get(roomEntity);
            pokerStartTimer.Timer += deltaTime;

            if (pokerStartTimer.Timer >= pokerStartTimer.TargetTime)
            {
                _pokerInitialize.Set(roomEntity);
                _pokerStartTimer.Remove(roomEntity);

                var dataframe = new PokerStartGameResetTimerDataframe();
                _server.SendInRoom(ref dataframe, roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}