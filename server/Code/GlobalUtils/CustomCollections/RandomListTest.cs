namespace server.Code.GlobalUtils.CustomCollections;

public static class RandomListTest
{
    public static void RunTest()
    {
        var cardDeck = new RandomList<string>(52)
        {
            "2♠", "3♠", "4♠", "5♠", "6♠", "7♠", "8♠", "9♠", "10♠", "J♠", "Q♠", "K♠", "A♠",
            "2♥", "3♥", "4♥", "5♥", "6♥", "7♥", "8♥", "9♥", "10♥", "J♥", "Q♥", "K♥", "A♥",
            "2♦", "3♦", "4♦", "5♦", "6♦", "7♦", "8♦", "9♦", "10♦", "J♦", "Q♦", "K♦", "A♦",
            "2♣", "3♣", "4♣", "5♣", "6♣", "7♣", "8♣", "9♣", "10♣", "J♣", "Q♣", "K♣", "A♣"
        };

        Console.WriteLine(new string('-', 30));
        Debug.LogColor($"before Count: {cardDeck.Count}", ConsoleColor.Cyan);
        Console.WriteLine(new string('-', 30));

        cardDeck.TryRandomRemove(out var card1);
        cardDeck.TryRandomRemove(out var card2);
        cardDeck.TryRandomRemove(out var card3);
        cardDeck.TryRandomRemove(out var card4);
        cardDeck.TryRandomRemove(out var card5);
        
        Debug.LogColor($"TABLE: {card1} {card2} {card3} {card4} {card5}", ConsoleColor.Yellow);
        
        Console.WriteLine(new string('-', 30));

        cardDeck.TryRandomRemove(out var hand1);
        cardDeck.TryRandomRemove(out var hand2);
        
        Debug.LogColor($"MEGA: {hand1} {hand2}", ConsoleColor.Yellow);
        
        Console.WriteLine(new string('-', 30));
        
        cardDeck.TryRandomRemove(out var hand3);
        cardDeck.TryRandomRemove(out var hand4);
        
        Debug.LogColor($"JUMBO: {hand3} {hand4}", ConsoleColor.Yellow);
        
        Console.WriteLine(new string('-', 30));
        Debug.LogColor($"after remove Count: {cardDeck.Count}", ConsoleColor.Cyan);
        Console.WriteLine(new string('-', 30));
        
        foreach (var card in cardDeck)
        {
            Debug.LogColor($"card in deck: {card}", ConsoleColor.Green);
        }
        
        cardDeck.Add(card1);
        cardDeck.Add(card2);
        
        Console.WriteLine(new string('-', 30));
        Debug.LogColor($"after add Count: {cardDeck.Count}", ConsoleColor.Cyan);
        Console.WriteLine(new string('-', 30));
        
        foreach (var card in cardDeck)
        {
            Debug.LogColor($"card in deck: {card}", ConsoleColor.Green);
        }
        Console.WriteLine(new string('-', 30));
    }
}