using Newtonsoft.Json;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;

namespace server.Code.MorpehFeatures.ConfigsFeature.Services;

public class ConfigsService : IInitializer
{
    [Injectable] private ServerParameters _serverParameters;
    
    public World World { get; set; }

    public void OnAwake()
    {
        
    }

    public T GetConfig<T>(string configPath) where T : class
    {
        var allPath = _serverParameters.ConfigPath + configPath;
        var json = File.ReadAllText(allPath);
        return  JsonConvert.DeserializeObject<T>(json);
    }

    public void Dispose()
    {
    }
}