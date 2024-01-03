using System.Diagnostics;

namespace chess_server.Code.GlobalUtils;

public static class Time
{
    private static Stopwatch _startWatch;
    private static float _deltaBase;
    
    public static float time => _startWatch.ElapsedMilliseconds / 1000f;
    public static float frameDeltaTime => time - _deltaBase;
    public static float deltaTime { get; private set; }

    public static void Initialize()
    {
        _startWatch = new Stopwatch();
        _startWatch.Start();
    }

    public static void UpdateDeltaBaseTime()
    {
        _deltaBase = time;
    }

    public static void FixedDeltaTime()
    {
        deltaTime = frameDeltaTime;
    }

    public static void UpdateDeltaTime()
    {
        FixedDeltaTime();
        UpdateDeltaBaseTime();
    }
}