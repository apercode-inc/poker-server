using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.GlobalUtils.CustomCollections;

public static class MovingMarkersDictionaryTest
{
    // public static void TestRun()
    // {
    //     var playerBySeat = new MovingMarkersDictionary<string, PokerPlayerMarkerType>(5);
    //     playerBySeat.Add(0, "Sex");
    //     playerBySeat.Add(1, "Kes");
    //     playerBySeat.Add(2, "Bet");
    //     playerBySeat.Add(3, "Meg");
    //     playerBySeat.Add(4, "POP");
    //     
    //     //playerBySeat.ShowMarkerTest(MarkersTestType.DealerPlayer);
    //     playerBySeat.SetMarker(4, PokerPlayerMarkerType.DealerPlayer);
    //     playerBySeat.SetMarker(2, PokerPlayerMarkerType.ActivePlayer);
    //     
    //     Console.WriteLine(new string('-', 30));
    //     foreach (var item in playerBySeat)
    //     {
    //         Console.WriteLine($"key = {item.Key} | value = {item.Value} | isDealer = {item.Markers[PokerPlayerMarkerType.DealerPlayer]} | isActive = {item.Markers[PokerPlayerMarkerType.ActivePlayer]}");
    //     }
    //     
    //     playerBySeat.TryMoveMarker(PokerPlayerMarkerType.DealerPlayer, out var currentDealerPlayer);
    //     playerBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out var currentActivePlayer);
    //
    //     Console.WriteLine(new string('-', 30));
    //     foreach (var item in playerBySeat)
    //     {
    //         Console.WriteLine($"key = {item.Key} | value = {item.Value} | isDealer = {item.Markers[PokerPlayerMarkerType.DealerPlayer]} | isActive = {item.Markers[PokerPlayerMarkerType.ActivePlayer]}");
    //     }
    //
    //     playerBySeat.Remove(0, out var removedItem);
    //
    //     Debug.LogColor($"After move: new dealer player = {currentDealerPlayer.Value}", ConsoleColor.Blue);
    //     
    //     Console.WriteLine(new string('-', 30));
    //     foreach (var item in playerBySeat)
    //     {
    //         Console.WriteLine($"key = {item.Key} | value = {item.Value} | isDealer = {item.Markers[PokerPlayerMarkerType.DealerPlayer]} | isActive = {item.Markers[PokerPlayerMarkerType.ActivePlayer]}");
    //     }
    //
    //     Console.WriteLine(new string('-', 30));
    //     Debug.LogColor($"RemovedItem = {removedItem.Value}", ConsoleColor.Blue);
    //
    //     if (playerBySeat.TryGetValueByMarked(PokerPlayerMarkerType.DealerPlayer, out var markedDealerPlayerBySeat))
    //     {
    //         Debug.LogColor($"Marked /{PokerPlayerMarkerType.DealerPlayer}/, player /{markedDealerPlayerBySeat.Value}/", ConsoleColor.Green);
    //     }
    //     else
    //     {
    //         Debug.LogColor($"No marked /{PokerPlayerMarkerType.DealerPlayer}/ player ", ConsoleColor.Yellow);
    //     }
    //     
    //     if (playerBySeat.TryGetValueByMarked(PokerPlayerMarkerType.ActivePlayer, out var markedActivePlayerBySeat))
    //     {
    //         Debug.LogColor($"Marked /{PokerPlayerMarkerType.ActivePlayer}/, player /{markedActivePlayerBySeat.Value}/", ConsoleColor.Green);
    //     }
    //     else
    //     {
    //         Debug.LogColor($"No marked /{PokerPlayerMarkerType.ActivePlayer}/ player ", ConsoleColor.Yellow);
    //     }
    //
    //
    //     Console.WriteLine(new string('-', 40));
    //     Console.WriteLine(new string('-', 40));
    //     Console.WriteLine(new string('-', 40));
    //     Console.WriteLine(new string('-', 40));
    // }
}