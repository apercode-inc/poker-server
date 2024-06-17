using System.Text;

namespace server.Code.GlobalUtils
{
    public static class ServerEnvsUtil
    {
        #if !PRODUCTION
            #if !_WINDOWS
        private const string DotenvFilePath = "~/poker-settings/.env";
            #else
        private const string DotenvFilePath = "C:/poker-settings/.env";
            #endif
        #else
        private const string DotenvFilePath = "/configs/.env";
        #endif
        private static readonly Dictionary<string, string> DotenvValues = new Dictionary<string, string>();
        
        static ServerEnvsUtil()
        {
            try {
                var path = DotenvFilePath;
                #if !PRODUCTION && !_WINDOWS
                path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                #endif
                var fileText = System.IO.File.ReadAllText(path, Encoding.UTF8);
                var lines = fileText.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    try
                    {
                        if (line.StartsWith("#"))
                            continue;
                        var pair = line.Split(new[] {'='}, StringSplitOptions.None);
                        DotenvValues[pair[0]] = pair[1];
                    }
                    catch
                    {
                        Logger.Error("Error reading line in dotenv");
                    }
                }
            }
            catch
            {
                Logger.Error("Unable to find dotenv file");
            }
        }

        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static object _lock = new object();

        public static string Read(string key, string defaultValue = null)
        {
            if (DotenvValues.ContainsKey(key))
            {
                return DotenvValues[key];
            }

            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    return value;
                }
                
                value = Environment.GetEnvironmentVariable(key);
                
                if (value == null)
                {
                    value = defaultValue;
                }
                else
                {
                    _cache[key] = value;
                }
                
                return value;
            }
        }
        
        public static bool ReadBool(string key, bool defaultValue = false)
        {
            var value = Read(key);
            if (value == null)
            {
                return defaultValue;
            }

            return value == "1" || value.ToLower() == "true";
        }
        
        public static int ReadInt(string key, int defaultValue = 0)
        {
            var value = Read(key);
            if (value == null)
            {
                return defaultValue;
            }
            
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}