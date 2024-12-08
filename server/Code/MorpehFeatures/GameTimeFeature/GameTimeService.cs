using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.GameTimeFeature;

public class GameTimeService : IInitializer
{
    private readonly DateTime _unixZero = DateTime.Parse("01.01.2000 00:00:00");
        
    public int CurrentTimeStamp => (int)(DateTime.Now - _unixZero).TotalSeconds;
    
    public World World { get; set; }

    public void OnAwake()
    {
        // TODO: time sync
    }
    
    public DateTime ToDateTime(int timestamp)
    {
        return _unixZero.AddSeconds(timestamp);
    }

    public void Dispose()
    {
        
    }
}