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

TestCombination();

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
    var combination = new RoomPokerCombinationSystem();

    var playerOneCards = new List<CardModel>
    {
        new(CardRank.Five, CardSuit.Spades),
        new(CardRank.Nine, CardSuit.Clubs),
    };

    var playerTwoCards = new List<CardModel>
    {
        new(CardRank.King, CardSuit.Hearts),
        new(CardRank.Jack, CardSuit.Hearts),
    };

    var tableCards = new List<CardModel>
    {
        new(CardRank.Queen, CardSuit.Diamonds),
        new(CardRank.Ten, CardSuit.Hearts),
        new(CardRank.Nine, CardSuit.Diamonds),
        new(CardRank.Seven, CardSuit.Hearts),
        new(CardRank.Two, CardSuit.Spades),
    };

    var resultPlayerOne = combination.DetermineCombination(playerOneCards, tableCards, 
        out var kickerRanksPlayerOne, out var advantageRankPlayerOne);

    var resultPlayerTwo = combination.DetermineCombination(playerTwoCards, tableCards, 
        out var kickerRanksPlayerTwo, out var advantageRankPlayerTwo);

    Debug.LogColor($"combination_player_1: {resultPlayerOne} ---> "
                   + Debug.GetCardsLog(playerOneCards) + "--- " + Debug.GetCardsLog(tableCards), ConsoleColor.Blue);

    Debug.LogColor($"advantageRankPlayerOne {advantageRankPlayerOne}", ConsoleColor.DarkCyan);
    foreach (var kicker in kickerRanksPlayerOne)
    {
        Debug.LogColor($"kicker_player_1: {kicker}", ConsoleColor.Yellow);
    }

    ///////
    
    Debug.LogColor($"combination_player_2: {resultPlayerTwo} ---> "
                   + Debug.GetCardsLog(playerTwoCards) + "--- " + Debug.GetCardsLog(tableCards), ConsoleColor.Blue);

    Debug.LogColor($"advantageRankPlayerTwo {advantageRankPlayerTwo}", ConsoleColor.DarkCyan);
    foreach (var kicker in kickerRanksPlayerTwo)
    {
        Debug.LogColor($"kicker_player_2: {kicker}", ConsoleColor.Yellow);
    }
}