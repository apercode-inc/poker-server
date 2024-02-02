using System.Diagnostics;
using NetFrame.Server;
using Scellecs.Morpeh;
using server;
using server.Code;
using server.Code.GlobalUtils;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;

//Injection
var container = new SimpleDImple();

//NetFrame
var server = new NetFrameServer(2000);
container.Register(server);

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

//todo test
MovingMarkersDictionaryTest.RunTest();

while (true)
{
    stopWatch.Reset();
    stopWatch.Start();
    
    //Console.WriteLine(Time.deltaTime);
    
    framesPerSecond++;
    Time.UpdateDeltaTime();
    
    //netFrameServer.Run(); //todo 
    systemExecutor.Execute();
    
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