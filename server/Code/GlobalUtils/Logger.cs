using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.GlobalUtils;

public static class Logger
{
    public static void Debug(string text, bool isSend = false)
    {
        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Debug);
        }
    }
    
    public static void DebugColor(string text, ConsoleColor color = ConsoleColor.Cyan, bool isSend = false)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Debug);
        }
    }
    
    public static void Error(string text, bool isSend = false)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Error);  
        }
    }

    public static void LogWarning(string text, bool isSend = false)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ResetColor();

        if (isSend)
        {
            SentrySdk.CaptureMessage(text, SentryLevel.Warning);  
        }
    }

    public static string GetCardsLog(List<CardModel> cardModels, string handSymbol = "")
    {
        var result = string.Empty;

        foreach (var card in cardModels)
        {
            result += GetCardLog(card, handSymbol) + " ";
        }

        return result;
    }

    public static string GetCardLog(CardModel cardModel, string handSymbol = "")
    {
        var result = string.Empty;

        var rank = cardModel.Rank;
        var suit = cardModel.Suit;
        var isHand = cardModel.IsHands;

        switch (rank)
        {
            case CardRank.Two:
                result += "2";
                break;
            case CardRank.Three:
                result += "3";
                break;
            case CardRank.Four:
                result += "4";
                break;
            case CardRank.Five:
                result += "5";
                break;
            case CardRank.Six:
                result += "6";
                break;
            case CardRank.Seven:
                result += "7";
                break;
            case CardRank.Eight:
                result += "8";
                break;
            case CardRank.Nine:
                result += "9";
                break;
            case CardRank.Ten:
                result += "10";
                break;
            case CardRank.Jack:
                result += "J";
                break;
            case CardRank.Queen:
                result += "Q";
                break;
            case CardRank.King:
                result += "K";
                break;
            case CardRank.Ace:
                result += "A";
                break;
        }
        
        switch (suit)
        {
            case CardSuit.Hearts:
                result += "♥";
                break;
            case CardSuit.Spades:
                result += "♠";
                break;
            case CardSuit.Diamonds:
                result += "♦";
                break;
            case CardSuit.Clubs:
                result += "♣";
                break;
        }

        if (isHand)
        {
            result += handSymbol;
        }

        return result;
    }
}