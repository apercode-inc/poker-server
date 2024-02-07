using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameStartTimerSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerGameStartTimer> _roomPokerGameStartTimer;
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerGameStartTimer>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.MarkedPlayersBySeat.Count < 2)
            {
                _roomPokerGameStartTimer.Remove(roomEntity);
                
                var dataframe = new RoomPokerStopGameResetTimerDataframe();
                _server.SendInRoom(ref dataframe, roomEntity);
            }
            
            ref var roomPokerGameStartTimer = ref _roomPokerGameStartTimer.Get(roomEntity);
            roomPokerGameStartTimer.Timer += deltaTime;

            if (roomPokerGameStartTimer.Timer >= roomPokerGameStartTimer.TargetTime)
            {
                _roomPokerGameInitialize.Set(roomEntity);
                _roomPokerGameStartTimer.Remove(roomEntity);

                var dataframe = new RoomPokerStartGameResetTimerDataframe();
                _server.SendInRoom(ref dataframe, roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}