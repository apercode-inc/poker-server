using server.Code.MorpehFeatures.PokerFeature.Enums;

namespace server.Code.GlobalUtils.CustomCollections;

public static class MovingMarkersDictionaryTest
{
    private static Dictionary<PokerPlayerMarkerType, string> _movedPlayersByMarker = new();
    
    public static void RunTest()
    {
        var playersBySeat = new MovingMarkersDictionary<string, PokerPlayerMarkerType>(5)
        {
            { 0, "Sex" },
            { 1, "Kes" },
            { 2, "Bet" },
            { 3, "Meg" },
            { 4, "POP" }
        };
        
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveWithRemoveForwardDirection, false);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveWithRemoveForwardDirection, true);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveForwardDirection, true);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveForwardDirection, true);
        
        playersBySeat.SetMarker(3, PokerPlayerMarkerType.DealerPlayer);
        playersBySeat.SetMarker(3, PokerPlayerMarkerType.ActivePlayer);
        
        foreach (var playerBySeat in playersBySeat)
        {
            Debug.LogColor($"seat: {playerBySeat.Key} | player: {playerBySeat.Value} | isDealer: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.DealerPlayer])} | isActive: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.ActivePlayer])}", ConsoleColor.Green);
        }

        //playersBySeat.TryMoveMarker(PokerPlayerMarkerType.DealerPlayer, out var newDealerPlayerByType);
        _movedPlayersByMarker.Clear();
        var removePlayer = "Meg";
        if (playersBySeat.Remove(removePlayer, _movedPlayersByMarker))
        {
            foreach (var playerNewMarked in _movedPlayersByMarker)
            {
                Debug.LogColor($"remove player: {removePlayer} | next marked player: {playerNewMarked.Value} | marker: {playerNewMarked.Key}" , ConsoleColor.Magenta);
            }
        }
        
        foreach (var playerBySeat in playersBySeat)
        {
            Debug.LogColor($"seat: {playerBySeat.Key} | player: {playerBySeat.Value} | isDealer: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.DealerPlayer])} | isActive: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.ActivePlayer])}", ConsoleColor.Green);
        }

        var moveMarker = PokerPlayerMarkerType.ActivePlayer;
        if (playersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out var nextMarkedPlayer))
        {
            Debug.LogColor($"next marked player: {nextMarkedPlayer.Value} | marker: {moveMarker}" , ConsoleColor.Magenta);
        }
        
        foreach (var playerBySeat in playersBySeat)
        {
            Debug.LogColor($"seat: {playerBySeat.Key} | player: {playerBySeat.Value} | isDealer: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.DealerPlayer])} | isActive: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.ActivePlayer])}", ConsoleColor.Green);
        }

        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
    }

    private static string GetFormat(bool value)
    {
        return value ? "X" : "O";
    }
}