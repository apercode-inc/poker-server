using System.Reflection;
using NetFrame.Enums;
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
        
        Logger.Debug("Server started...");

        _server.ClientConnection += OnClientConnection;
        _server.ClientDisconnect += OnClientDisconnect;
        _server.LogCall += OnCallLog;
    }

    public void OnUpdate(float deltaTime)
    {
        _server.Run(100);
    }

    private void OnClientConnection(int id)
    {
        var currentDateTime = DateTime.Now;
        var ipAddress = _server.GetClientAddress(id);
        Logger.Debug($"[{currentDateTime}] connected player ip = {ipAddress}, id = {id}");
        
        _playerStorage.Add(id);
    }

    private void OnClientDisconnect(int id)
    {
        var currentDateTime = DateTime.Now;
        Logger.Debug($"[{currentDateTime}] disconnected player, id = {id}");
        
        _playerStorage.RemoveWithAway(id);
    }
    
    private void OnCallLog(NetworkLogType logType, string message)
    {
        switch (logType)
        {
            case NetworkLogType.Info:
                Logger.Debug(message);
                break;
            case NetworkLogType.Warning:
                Logger.LogWarning(message);
                break;
            case NetworkLogType.Error:
                Logger.Error(message);
                break;
        }
    }

    public void Dispose()
    {
        _server.Stop();
        
        _server.ClientConnection -= OnClientConnection;
        _server.ClientDisconnect -= OnClientDisconnect;
        _server.LogCall -= OnCallLog;
    }
}