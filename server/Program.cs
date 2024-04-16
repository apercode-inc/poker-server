using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NetFrame.Server;
using Scellecs.Morpeh;
using Sentry;
using Sentry.Extensions.Logging;
using server;
using server.Code;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;

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
    var roomPokerCombinationSystem = new RoomPokerDetectCombinationSystem();

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

    var tableCards = new List<CardModel>
    {
        new(CardRank.Two, CardSuit.Hearts),
        new(CardRank.Five, CardSuit.Clubs),
        new(CardRank.Ten, CardSuit.Spades),
        new(CardRank.Three, CardSuit.Spades),
        new(CardRank.Four, CardSuit.Hearts),
    };
    
    Logger.Debug("------------ Hands Player_2 ------------", ConsoleColor.Magenta);
    Logger.Debug($"{Logger.GetCardsLog(playerTwoCards)}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Hands Player_1 ------------", ConsoleColor.Magenta);
    Logger.Debug($"{Logger.GetCardsLog(playerOneCards)}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Table Cards ------------", ConsoleColor.Magenta);
    Logger.Debug($"{Logger.GetCardsLog(tableCards)}", ConsoleColor.Cyan);
    
    var roomPokerCombinationCompareSystem = new RoomPokerCombinationCompareSystem();
    
    var compareTestCards = new List<CardModel>
    {
        new(CardRank.Queen, CardSuit.Hearts),
        new(CardRank.Jack, CardSuit.Clubs),
        new(CardRank.Jack, CardSuit.Spades),
        new(CardRank.Three, CardSuit.Spades),
        new(CardRank.Three, CardSuit.Hearts),
    };

    roomPokerCombinationCompareSystem.SortCombinationAndKickers(compareTestCards);
    
    Console.WriteLine();
    
    
    Logger.Debug("------------ Combination Player_2 ------------", ConsoleColor.Magenta);
    
    // //calculate player_2
    // var combinationTwoPlayer = roomPokerCombinationSystem.GetPokerCombination(playerTwoCards, tableCards, 
    //     out var combinationOrdersCardsTwoPlayer);
    //
    // Logger.Debug($"{Logger.GetCardsLog(combinationOrdersCardsTwoPlayer, "^")}", ConsoleColor.Cyan);
    // Logger.Debug($"{combinationTwoPlayer}", ConsoleColor.Cyan);
    //
    // Logger.Debug("------------ Combination Player_1 ------------", ConsoleColor.Magenta);
    //
    // //calculate player_
    // var combinationOnePlayer = roomPokerCombinationSystem.GetPokerCombination(playerOneCards, tableCards, 
    //     out var combinationOrdersCardsOnePlayer);
    //
    // Logger.Debug($"{Logger.GetCardsLog(combinationOrdersCardsOnePlayer, "^")}", ConsoleColor.Cyan);
    // Logger.Debug($"{combinationOnePlayer}", ConsoleColor.Cyan);
}