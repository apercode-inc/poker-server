using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckStopGameSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerActive> _pokerActive;

    [Injectable] private NetFrameServer _server;
    
    public World World { get; set; }

    private Filter _filter;

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerActive>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.MarkedPlayersBySeat.Count < 2)
            {
                _pokerActive.Remove(roomEntity);
                
                //todo тут также должна быть логика по здаче карт в колоду и т.д
                
                var dataframe = new RoomPokerStopGameResetTimerDataframe();
                _server.SendInRoom(ref dataframe, roomEntity);
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}