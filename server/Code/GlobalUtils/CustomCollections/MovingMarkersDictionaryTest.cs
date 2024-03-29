using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.GlobalUtils.CustomCollections;

public static class MovingMarkersDictionaryTest
{
    private static Dictionary<PokerPlayerMarkerType, PlayerEntity> _movedPlayersByMarker = new();
    
    public static void RunTest()
    {
        var sexPlayer = new PlayerEntity("Sex");
        var kesPlayer = new PlayerEntity("Kes");
        var betPlayer = new PlayerEntity("Rim");
        var megPlayer = new PlayerEntity("Meg");
        var popPlayer = new PlayerEntity("Pop");
        
        var playersBySeat = new MovingMarkersDictionary<PlayerEntity, PokerPlayerMarkerType>(5)
        {
            { 0, sexPlayer },
            { 1, kesPlayer },
            { 2, betPlayer },
            { 3, megPlayer },
            { 4, popPlayer },
        };
        
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveWithRemoveForwardDirection, false);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveWithRemoveForwardDirection, true);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveForwardDirection, true);
        playersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveForwardDirection, true);
        
        playersBySeat.SetMarker(megPlayer, PokerPlayerMarkerType.DealerPlayer);
        playersBySeat.SetMarker(megPlayer, PokerPlayerMarkerType.ActivePlayer);
        
        ShowAllPlayers(playersBySeat);

        //playersBySeat.TryMoveMarker(PokerPlayerMarkerType.DealerPlayer, out var newDealerPlayerByType);
        _movedPlayersByMarker.Clear();
        var removePlayer = megPlayer;
        if (playersBySeat.Remove(removePlayer, _movedPlayersByMarker))
        {
            Logger.Debug($"remove player: {removePlayer.Nickname}", ConsoleColor.Magenta);
            foreach (var playerNewMarked in _movedPlayersByMarker)
            {
                Logger.Debug($"next marked {playerNewMarked.Key}: {playerNewMarked.Value.Nickname}" , ConsoleColor.Magenta);
            }
        }
        
        ShowAllPlayers(playersBySeat);

        var moveMarker = PokerPlayerMarkerType.ActivePlayer;
        if (playersBySeat.TryMoveMarker(PokerPlayerMarkerType.ActivePlayer, out var nextMarkedPlayer))
        {
            Logger.Debug($"Move Marker {moveMarker}, next marked player: {nextMarkedPlayer.Value.Nickname}" , ConsoleColor.Magenta);
        }
        
        ShowAllPlayers(playersBySeat);

        var resetMarker = PokerPlayerMarkerType.ActivePlayer;
        playersBySeat.ResetMarker(PokerPlayerMarkerType.ActivePlayer);
        
        Logger.Debug($"Reset marked {resetMarker}", ConsoleColor.Magenta);
        
        ShowAllPlayers(playersBySeat);
        
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(new string('-', 40));
    }

    private static void ShowAllPlayers(MovingMarkersDictionary<PlayerEntity, PokerPlayerMarkerType> playersBySeat)
    {
        foreach (var playerBySeat in playersBySeat)
        {
            Logger.Debug(
                $"seat: {playerBySeat.Key} | player: {playerBySeat.Value.Nickname} | isDealer: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.DealerPlayer])} | isActive: {GetFormat(playerBySeat.Markers[PokerPlayerMarkerType.ActivePlayer])}",
                ConsoleColor.Green);
        }
    }

    private static string GetFormat(bool value)
    {
        return value ? "X" : "O";
    }
}

public class PlayerEntity
{
    public readonly string Nickname;
    
    public PlayerEntity(string nickname)
    {
        Nickname = nickname;
    }
}