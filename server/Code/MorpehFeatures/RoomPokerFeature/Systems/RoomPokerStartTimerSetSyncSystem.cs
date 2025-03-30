using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerStartTimerSetSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    
    [Injectable] private Stash<RoomPokerGameStartTimer> _pokerStartTimer;
    [Injectable] private Stash<RoomPokerActive> _pokerActive;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<RoomPokerStartGameSetTimerRequestDataframe>(Handler);
    }

    private void Handler(RoomPokerStartGameSetTimerRequestDataframe dataframe, int playerId)
    {
        if (!_playerStorage.TryGetPlayerById(playerId, out var player))
        {
            return;
        }
        
        ref var playerRoomPoker = ref _playerRoomPoker.Get(player, out var exist);

        if (!exist)
        {
            return;
        }

        var room = playerRoomPoker.RoomEntity;

        ref var pokerStartTimer = ref _pokerStartTimer.Get(room, out var timerExist);

        if (timerExist)
        {
            var responseSetTimerDataframe = new RoomPokerStartGameSetTimerDataframe
            {
                WaitTime = pokerStartTimer.TargetTime - pokerStartTimer.Timer,
            };
            _server.SendInRoom(ref responseSetTimerDataframe, room);
        }
        else if(_pokerActive.Has(room))
        {
            var responseResetTimer = new RoomPokerStartGameResetTimerDataframe();
            _server.SendInRoom(ref responseResetTimer, room);
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<RoomPokerStartGameSetTimerRequestDataframe>(Handler);
    }
}