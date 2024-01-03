using System.Reflection;
using NetFrame.Server;
using NetFrame.Utils;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;

namespace server.Code.MorpehFeatures.ConnectionFeature.Systems;

public class StartServerSystem : ISystem
{
    [Injectable] private NetFrameServer _server;
    //[Injectable] private PlayerStorageSystem _playerStorage;

    private const int MaxPlayers = 10;
    private const int Port = 8080;
    
    public World World { get; set; }

    public void OnAwake()
    {
        NetFrameDataframeCollection.Initialize(Assembly.GetExecutingAssembly());
        
        _server.Start(Port, MaxPlayers);
        Debug.LogColor("Server started...", ConsoleColor.Green);

        _server.ClientConnection += OnClientConnection;
        _server.ClientDisconnect += OnClientDisconnect;
    }

    private void OnClientConnection(int id)
    {
        Debug.LogColor($"connected player id = {id}", ConsoleColor.Green);
        //_playerStorage.Add(id);
    }
    
    private void OnClientDisconnect(int id)
    {
        Debug.LogColor($"disconnected player id = {id}", ConsoleColor.Yellow);
        //_playerStorage.Remove(id);
    }

    public void OnUpdate(float deltaTime)
    {
        _server.Run(100);
    }

    public void Dispose()
    {
        _server.Stop();
        
        _server.ClientConnection -= OnClientConnection;
        _server.ClientDisconnect -= OnClientDisconnect;
    }
}