using System.Diagnostics;
using NetFrame.Server;
using Scellecs.Morpeh;
using server;
using server.Code;
using server.Code.GlobalUtils;
using server.Code.Injection;

//Injection
var container = new SimpleDImple();

//NetFrame
var server = new NetFrameServer(2000);
container.Register(server);

//.Server parameters
var serverParameters = new ServerParameters
{
    IsProduction = ServerEnvsUtil.ReadBool("PRODUCTION"),
    Version = "0.0.1",
    Port = ServerEnvsUtil.ReadInt("SERVER_PORT"),
    MaxPlayers = ServerEnvsUtil.ReadInt("MAX_PLAYERS"),
    ConfigPath = ServerEnvsUtil.Read("CONFIG_PATH"),
    SentryDsn = ServerEnvsUtil.Read("SENTRY_DSN"),
    SqlHost = ServerEnvsUtil.Read("MYSQL_HOST"),
    SqlPort = int.Parse(ServerEnvsUtil.Read("MYSQL_PORT", "3306")),
    SqlUser = ServerEnvsUtil.Read("MYSQL_USER"),
    SqlPassword = ServerEnvsUtil.Read("MYSQL_PASSWORD"),
    SqlDatabase = ServerEnvsUtil.Read("MYSQL_DATABASE")
};
container.Register(serverParameters);

//Morpeh ECS
WorldExtensions.InitializationDefaultWorld();
var world = World.Default;
container.Register(world);
MorpehInitializer.Initialize(world, container);
var systemExecutor = new SystemsExecutor(world);

//TimeCycle
var stopWatch = new Stopwatch();
var targetMilliseconds = 5d;
var frameRateTimer = 0f;
var framesPerSecond = 0;

Time.Initialize();

//Sentry Init
using (SentrySdk.Init(options =>
       {
           options.Dsn = serverParameters.SentryDsn;
           options.Debug = true;
           options.TracesSampleRate = 0.0f;
           options.SampleRate = 1.0f;
           options.Release = serverParameters.Version;
           options.Environment = serverParameters.IsProduction ? "prod" : "dev";
       }))
    
MainThread.Assert();
    
while (true)
{
    stopWatch.Reset();
    stopWatch.Start();

    //Console.WriteLine(Time.deltaTime);
    
    framesPerSecond++;
    Time.UpdateDeltaTime();
    
    //netFrameServer.Run(); //todo 
    systemExecutor.Execute();
    
    MainThread.Pulse();
    
    frameRateTimer += Time.deltaTime;
    
    if (frameRateTimer >= 1f)
    {
        //Debug.LogColor($"Global Server FPS: {framesPerSecond}", ConsoleColor.Green);
        framesPerSecond = 0;
        frameRateTimer = 0f;
    }
    
    stopWatch.Stop();

    var timeLeft = (int) (targetMilliseconds - stopWatch.Elapsed.TotalMilliseconds);

    if (timeLeft > 0)
    {
        Thread.Sleep(timeLeft);
    }
}