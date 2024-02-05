using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.PokerFeature.Dataframes.StartTimer;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerStartTimerSetSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    
    [Injectable] private Stash<PokerStartTimer> _pokerStartTimer;
    [Injectable] private Stash<PokerActive> _pokerActive;
    
    [Injectable] private NetFrameServer _server;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private RoomPokerStorage _roomPokerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<PokerStartGameSetTimerRequestDataframe>(Handler);
    }

    private void Handler(PokerStartGameSetTimerRequestDataframe dataframe, int playerId)
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

        if (!_roomPokerStorage.TryGetById(playerRoomPoker.RoomId, out var room))
        {
            return;
        }

        ref var pokerStartTimer = ref _pokerStartTimer.Get(room, out var timerExist);

        if (timerExist)
        {
            var responseSetTimerDataframe = new PokerStartGameSetTimerDataframe
            {
                WaitTime = pokerStartTimer.TargetTime - pokerStartTimer.Timer,
            };
            _server.SendInRoom(ref responseSetTimerDataframe, room);
        }
        else if(_pokerActive.Has(room))
        {
            var responseResetTimer = new PokerStartGameResetTimerDataframe();
            _server.SendInRoom(ref responseResetTimer, room);
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<PokerStartGameSetTimerRequestDataframe>(Handler);
    }
}