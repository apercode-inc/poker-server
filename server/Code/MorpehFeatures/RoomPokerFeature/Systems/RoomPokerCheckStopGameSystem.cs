using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckStopGameSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerActive> _pokerActive;

    [Injectable] private NetFrameServer _server;
    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;
    
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

                foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
                {
                    var player = playerBySeat.Value;
                    _roomPokerCardDeskService.ReturnCardInDesk(roomEntity, player);
                }
                
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