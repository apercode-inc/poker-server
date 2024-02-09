using Newtonsoft.Json;
using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.ConfigsFeature.Services;

public class ConfigsService : IInitializer
{
    public World World { get; set; }

    private string _projectPath;

    public void OnAwake()
    {
        _projectPath = GetProjectPath();
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