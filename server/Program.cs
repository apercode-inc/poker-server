using System.Diagnostics;
using NetFrame.Server;
using Scellecs.Morpeh;
using server;
using server.Code;
using server.Code.GlobalUtils;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;
using Debug = server.Code.GlobalUtils.Debug;

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
//MovingMarkersDictionaryTest.RunTest();

//TODO test
var combination = new RoomPokerCombinationSystem();

var playerCards = new List<CardModel>
{
    new(CardRank.Ace, CardSuit.Diamonds),
    new(CardRank.King, CardSuit.Diamonds),
};

var tableCards = new List<CardModel>
{
    new(CardRank.Queen, CardSuit.Diamonds),
    new(CardRank.Jack, CardSuit.Spades),
    new(CardRank.Ten, CardSuit.Diamonds),
    new(CardRank.Two, CardSuit.Hearts),
    new(CardRank.Three, CardSuit.Clubs),
};

var result = combination.DetermineCombination(playerCards, tableCards);
Debug.LogColor($"{result}", ConsoleColor.Blue);

//TODO end

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