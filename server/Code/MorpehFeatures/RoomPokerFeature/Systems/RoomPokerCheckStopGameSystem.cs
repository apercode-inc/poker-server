using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckStopGameSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCleanup> _roomPokerCleanup;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .Without<RoomPokerCleanedGame>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.TotalPlayersCount != 1)
            {
                continue;
            }
            
            _roomPokerPayoutWinnings.Set(roomEntity);
            _roomPokerCleanup.Set(roomEntity);
            
            _roomPokerActive.Remove(roomEntity);
            
            ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);
            roomPokerBank.OnTable = roomPokerBank.Total;
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}