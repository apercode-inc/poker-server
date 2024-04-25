using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomChatFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.RoomChatFeature.Systems;

public class RoomChatMessagesSyncInitializer : IInitializer
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PlayerId> _playerId;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    private Filter _filter;

    public World World { get; set; }
    
    public void OnAwake()
    {
        _server.Subscribe<ChatMessageDataframe>(OnChatMessage);
    }

    private void OnChatMessage(ChatMessageDataframe message, int senderId)
    {
        if (!_playerStorage.TryGetPlayerById(senderId, out var player))
        {
            return;
        }

        ref var room = ref _playerRoomPoker.Get(player, out bool exist);
        if (!exist)
        {
            return;
        }

        _server.SendInRoomExcept(ref message, room.RoomEntity, player);
    }

    public void Dispose()
    {
    }
}