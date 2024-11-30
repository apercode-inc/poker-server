using Newtonsoft.Json;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;

namespace server.Code.MorpehFeatures.ConfigsFeature.Services;

public class ConfigsService : IInitializer
{
    [Injectable] private ServerParameters _serverParameters;

    private Dictionary<string, object> _configsCache;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _configsCache = new Dictionary<string, object>();
    }

    public T GetConfig<T>(string configPath) where T : class
    {
        if (_configsCache.TryGetValue(configPath, out var config) && config is T typedConfig)
        {
            return typedConfig;
        }
        
        var allPath = _serverParameters.ConfigPath + configPath;
        var json = File.ReadAllText(allPath);
        
        typedConfig = JsonConvert.DeserializeObject<T>(json);
        _configsCache[configPath] = typedConfig;
        
        return typedConfig;
    }

    public void Dispose()
    {
    }
}