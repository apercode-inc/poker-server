using Newtonsoft.Json;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;

namespace server.Code.MorpehFeatures.ConfigsFeature.Services;

public class ConfigsService : IInitializer
{
    public World World { get; set; }

    private string _projectPath;

    public void OnAwake()
    {
        Debug.LogError(GetProjectPath());

        _projectPath = GetProjectPath();
        
        var config = GetConfig<RoomPokerTimers>(ConfigsPath.PokerGameTimers);
        Debug.LogColor($"StartGameTime = {config.StartGameTime} | PlayerTurnTime = {config.PlayerTurnTime}", ConsoleColor.Green);
    }

    public T GetConfig<T>(string configPath) where T : class
    {
        var allPath = _projectPath + configPath;
        var json = File.ReadAllText(allPath);
        return  JsonConvert.DeserializeObject<T>(json);
    }
    
    private string GetProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;
        return projectDirectory;
    }

    public void Dispose()
    {
    }
}