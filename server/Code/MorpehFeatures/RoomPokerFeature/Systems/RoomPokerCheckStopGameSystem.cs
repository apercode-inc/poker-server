using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCheckStopGameSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;
    [Injectable] private Stash<RoomPokerCleanupTimer> _roomPokerCleanupTimer;
    [Injectable] private Stash<RoomPokerShowOrHideCards> _roomPokerShowOrHideCards;
    
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

            if (roomPokerPlayers.MarkedPlayersBySeat.Count >= 2)
            {
                continue;
            }
            
            _roomPokerActive.Remove(roomEntity);

            var playerGivenBank = new FastList<Entity>();

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = playerBySeat.Value;

                playerGivenBank.Add(player);
            }
            
            _roomPokerPlayersGivenBank.Set(roomEntity, new RoomPokerPlayersGivenBank
            {
                Players = playerGivenBank
            });

            var dataframe = new RoomPokerStopGameResetTimerDataframe();
            _server.SendInRoom(ref dataframe, roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}