using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationAuthDataSetSystem : ISystem
{
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerAway> _playerAway;
    [Injectable] private Stash<PlayerAwayRejoinRoom> _playerAwayRejoinRoom;

    [Injectable] private ThreadSafeFilter<UserLoadCompleteSafeContainer> _loadCompleteSafeFilter;

    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private NetFrameServer _server;

    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var safeContainer in _loadCompleteSafeFilter)
        {
            if (_playerStorage.TryGetPlayerByGuid(safeContainer.PlayerGuid, out var player) && _playerAway.Has(player))
            {
                _playerAwayRejoinRoom.Set(player, new PlayerAwayRejoinRoom
                {
                    NewId = safeContainer.PlayerId,
                });
                continue;
            }

            if (!_playerStorage.TryGetPlayerById(safeContainer.PlayerId, out player))
            {
                continue;
            }

            _playerStorage.AddAuth(player, safeContainer.PlayerGuid, safeContainer.PlayerId);
            
            _playerDbModelRequest.Set(player);
        }
    }

    public void Dispose()
    {
    }
}