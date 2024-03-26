using System.Diagnostics;
using NetFrame.Server;
using Scellecs.Morpeh;
using server;
using server.Code;
using server.Code.GlobalUtils;
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

//TestCombination();

//TODO test

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

void TestCombination()
{
    var roomPokerCombinationSystem = new RoomPokerCombinationSystem();

    var playerTwoCards = new List<CardModel>
    {
        new(CardRank.Queen, CardSuit.Diamonds) { IsHands = true },
        new(CardRank.Seven, CardSuit.Spades) { IsHands = true },
    };
    
    var playerOneCards = new List<CardModel>
    {
        new(CardRank.Ace, CardSuit.Hearts) { IsHands = true },
        new(CardRank.Seven, CardSuit.Diamonds) { IsHands = true },
    };

    var tableCards = new List<CardModel> //A, 2, 3, 4, 5 не учитывает младший стрит (колесо)
    {
        new(CardRank.Two, CardSuit.Hearts),
        new(CardRank.Jack, CardSuit.Clubs),
        new(CardRank.Ten, CardSuit.Spades),
        new(CardRank.Nine, CardSuit.Spades),
        new(CardRank.Four, CardSuit.Hearts),
    };
    
    Debug.LogColor("------------ Hands Player_2 ------------", ConsoleColor.Magenta);
    Debug.LogColor($"{Debug.GetCardsLog(playerTwoCards)}", ConsoleColor.Cyan);
    
    Debug.LogColor("------------ Hands Player_1 ------------", ConsoleColor.Magenta);
    Debug.LogColor($"{Debug.GetCardsLog(playerOneCards)}", ConsoleColor.Cyan);
    
    Debug.LogColor("------------ Table Cards ------------", ConsoleColor.Magenta);
    Debug.LogColor($"{Debug.GetCardsLog(tableCards)}", ConsoleColor.Cyan);

    
    Console.WriteLine();


    Debug.LogColor("------------ Combination Player_2 ------------", ConsoleColor.Magenta);
    
    //calculate player_2
    var combinationTwoPlayer = roomPokerCombinationSystem.GetPokerCombination(playerTwoCards, tableCards, 
        out var combinationOrdersCardsTwoPlayer);

    Debug.LogColor($"{Debug.GetCardsLog(combinationOrdersCardsTwoPlayer, "^")}", ConsoleColor.Cyan);
    Debug.LogColor($"{combinationTwoPlayer}", ConsoleColor.Cyan);

    Debug.LogColor("------------ Combination Player_1 ------------", ConsoleColor.Magenta);
    
    //calculate player_
    var combinationOnePlayer = roomPokerCombinationSystem.GetPokerCombination(playerOneCards, tableCards, 
        out var combinationOrdersCardsOnePlayer);
    
    Debug.LogColor($"{Debug.GetCardsLog(combinationOrdersCardsOnePlayer, "^")}", ConsoleColor.Cyan);
    Debug.LogColor($"{combinationOnePlayer}", ConsoleColor.Cyan);
}