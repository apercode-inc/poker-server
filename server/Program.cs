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

    var playerOneCards = new List<CardModel> //Vasya
    {
        new(CardRank.King, CardSuit.Clubs) { IsHands = true },
        new(CardRank.Ace, CardSuit.Diamonds) { IsHands = true },
    };
    
    var playerTwoCards = new List<CardModel> //Evgeniy
    {
        new(CardRank.King, CardSuit.Clubs) { IsHands = true },
        new(CardRank.Seven, CardSuit.Hearts) { IsHands = true },
    };
    
    var playerThreeCards = new List<CardModel> //Kate
    {
        new(CardRank.Nine, CardSuit.Hearts) { IsHands = true },
        new(CardRank.Nine, CardSuit.Clubs) { IsHands = true },
    };

    var tableCards = new List<CardModel>
    {
        new(CardRank.Seven, CardSuit.Clubs),
        new(CardRank.Two, CardSuit.Spades),
        new(CardRank.Seven, CardSuit.Diamonds),
        new(CardRank.Nine, CardSuit.Spades),
        new(CardRank.King, CardSuit.Diamonds),
    };

    Logger.Debug("------------ Hands Player_1 ------------", ConsoleColor.Magenta);
    Logger.Debug($"Vasya:   {Logger.GetCardsLog(playerOneCards)}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Hands Player_2 ------------", ConsoleColor.Magenta);
    Logger.Debug($"Evgeniy: {Logger.GetCardsLog(playerTwoCards)}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Hands Player_3------------", ConsoleColor.Magenta);
    Logger.Debug($"Kate:    {Logger.GetCardsLog(playerThreeCards)}", ConsoleColor.Cyan);

    Logger.Debug("------------ Table Cards ------------", ConsoleColor.Magenta);
    Logger.Debug($"{Logger.GetCardsLog(tableCards)}", ConsoleColor.Cyan);

    Console.WriteLine();
    
    Logger.Debug("------------ Combination Player_1 ------------", ConsoleColor.Magenta);
    
    //calculate player_
    var combinationOnePlayer = roomPokerCombinationSystem.GetPokerCombination(playerOneCards, tableCards, 
        out var combinationOrdersCardsOnePlayer);
    
    Logger.Debug($"{Logger.GetCardsLog(combinationOrdersCardsOnePlayer, "^")}", ConsoleColor.Cyan);
    Logger.Debug($"Vasya:   {combinationOnePlayer}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Combination Player_2 ------------", ConsoleColor.Magenta);
    
    //calculate player_2
    var combinationTwoPlayer = roomPokerCombinationSystem.GetPokerCombination(playerTwoCards, tableCards, 
        out var combinationOrdersCardsTwoPlayer);
    
    Logger.Debug($"{Logger.GetCardsLog(combinationOrdersCardsTwoPlayer, "^")}", ConsoleColor.Cyan);
    Logger.Debug($"Evgeniy: {combinationTwoPlayer}", ConsoleColor.Cyan);
    
    Logger.Debug("------------ Combination Player_3 ------------", ConsoleColor.Magenta);
    
    //calculate player_3
    var combinationThreePlayer = roomPokerCombinationSystem.GetPokerCombination(playerThreeCards, tableCards, 
        out var combinationOrdersCardsThreePlayer);
    
    Logger.Debug($"{Logger.GetCardsLog(combinationOrdersCardsThreePlayer, "^")}", ConsoleColor.Cyan);
    Logger.Debug($"Kate:    {combinationThreePlayer}", ConsoleColor.Cyan);
    
    var playerByCardsCombinations = new Dictionary<Player, List<CardModel>>
    {
        [new Player("Vasya", 27, combinationOnePlayer)] = combinationOrdersCardsOnePlayer,
        [new Player("Evgeniy", 32, combinationTwoPlayer)] = combinationOrdersCardsTwoPlayer,
        [new Player("Kate", 25, combinationThreePlayer)] = combinationOrdersCardsThreePlayer,
    };

    //Max combination
    var maxCombination = CombinationType.HighCard;
    var combinations = new List<CombinationType>
    {
        combinationOnePlayer, combinationTwoPlayer, combinationThreePlayer,
    };
    
    foreach (var combination in combinations)
    {
        if (combination > maxCombination)
        {
            maxCombination = combination;
        }
    }

    var removedPlayers = new List<Player>();

    foreach (var playerByCards in playerByCardsCombinations)
    {
        if (playerByCards.Key.CombinationType != maxCombination)
        {
            removedPlayers.Add(playerByCards.Key);
        }
    }

    foreach (var removedPlayer in removedPlayers)
    {
        playerByCardsCombinations.Remove(removedPlayer);
    }

    Logger.Debug($"MAX combination: {maxCombination}", ConsoleColor.Magenta);

    var roomPokerCombinationCompareSystem = new RoomPokerCombinationCompareSystem();

    var winningPlayers = roomPokerCombinationCompareSystem.DefineWinningPlayersByCombination(maxCombination, playerByCardsCombinations);


    foreach (var winningPlayer in winningPlayers)
    {
        Logger.Debug($"WIN PLAYER ---> Nickname: {winningPlayer.Nickname} | Age: {winningPlayer.Age}", ConsoleColor.Yellow);
    }
    
}

public class Player
{
    public string Nickname;
    public int Age;
    public CombinationType CombinationType;

    public Player(string nickname, int age, CombinationType combinationType)
    {
        Nickname = nickname;
        Age = age;
        CombinationType = combinationType;
    }
}