using System.Reflection;
using NetFrame.Server;
using NetFrame.Utils;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.ConnectionFeature.Systems;

public class StartServerSystem : ISystem
{
    [Injectable] private ServerParameters _serverParameters;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        NetFrameDataframeCollection.Initialize(Assembly.GetExecutingAssembly());

        _server.SetProtectionWithFilePath(_serverParameters.PrivateKeyPath, _serverParameters.SecretToken);
        _server.Start(_serverParameters.Port, _serverParameters.MaxPlayers);
        
        Logger.DebugColor("Server started...", ConsoleColor.Green);

        _server.ClientConnection += OnClientConnection;
        _server.ClientDisconnect += OnClientDisconnect;
    }
    
    public void OnUpdate(float deltaTime)
    {
        _server.Run(100);
    }

    private void OnClientConnection(int id)
    {
        Logger.DebugColor($"connected player id = {id}", ConsoleColor.Green);
        _playerStorage.Add(id);
    }
    
    private void OnClientDisconnect(int id)
    {
        Logger.DebugColor($"disconnected player id = {id}", ConsoleColor.Yellow);
        _playerStorage.Remove(id);
    }

    public void Dispose()
    {
        _server.Stop();
        
        _server.ClientConnection -= OnClientConnection;
        _server.ClientDisconnect -= OnClientDisconnect;
    }
}