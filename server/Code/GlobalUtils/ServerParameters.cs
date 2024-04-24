using server.Code.MorpehFeatures.ConfigsFeature.Constants;

namespace server.Code.GlobalUtils;

public class ServerParameters
{
    public bool IsProduction;
    public string Version;
    public int Port;
    public int MaxPlayers;
    public string ConfigPath;
    public string SentryDsn;
    public string SqlHost;
    public int SqlPort;
    public string SqlUser;
    public string SqlPassword;
    public string SqlDatabase;
}