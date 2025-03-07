using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationDisconnectAlreadyConnectedSystem : ILateSystem
{
    [Injectable] private Stash<AuthenticationDisconnectAlreadyConnected> _authenticationDisconnectAlreadyConnected;
    [Injectable] private Stash<PlayerId> _playerId;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<AuthenticationDisconnectAlreadyConnected>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var playerId = ref _playerId.Get(entity);
            _server.Disconnect(playerId.Id);
            _playerStorage.Remove(playerId.Id);
            _authenticationDisconnectAlreadyConnected.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}