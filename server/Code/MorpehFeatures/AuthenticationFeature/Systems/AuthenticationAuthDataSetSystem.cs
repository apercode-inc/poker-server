using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Components;
using server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationAuthDataSetSystem : ISystem
{
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<AuthenticationDisconnectAlreadyConnected> _authenticationDisconnectAlreadyConnected;
    
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
            if (!_playerStorage.TryGetPlayerById(safeContainer.PlayerId, out var player))
            {
                continue;
            }

            if (_playerStorage.TryGetPlayerByGuid(safeContainer.PlayerGuid, out _))
            {
                var dataframe = new AuthenticationPlayerAlreadyConnectedDataframe();
                _server.Send(ref dataframe, player);
                _authenticationDisconnectAlreadyConnected.Set(player);
                continue;
            }
            
            _playerStorage.AddAuth(player, safeContainer.PlayerGuid);
            _playerDbModelRequest.Set(player);
        }
    }

    public void Dispose()
    {
    }
}