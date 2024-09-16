using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPayoutWinningsSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    private List<PlayerPotModel> _playerPotModelWinners;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _playerPotModelWinners = new List<PlayerPotModel>();
        
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<RoomPokerPayoutWinnings>()
            .Build();
    }

    public void Dispose()
    {
        _filter = null;
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            var players = roomPokerPlayers.PlayerPotModels.Values;
            
            while (PotChipsRemaining(players.ToList()) > 0)
            {
                PayOutWinners(CalculateAndSortWinners(players.ToList()), players.ToList());
            }

            // Refund players if remaining chips in pot (bigger/folded stacks)
            foreach (var player in players)
            {
                if (!player.IsFold || player.PotCommitment == 0)
                {
                    continue;
                }

                player.ChipsRemaining += player.PotCommitment;
                player.PotCommitment = 0;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Refund ---> Player: {player.Guid} chips: {player.ChipsRemaining} paid out.");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"*********************** Final results:");
            Console.ResetColor();

            PotChipsRemaining(players.ToList());
            
            _roomPokerPayoutWinnings.Remove(roomEntity);
        }
    }

    private List<PlayerPotModel> CalculateAndSortWinners(List<PlayerPotModel> playersInHand)
    {
        int highHand = 0;
        // Get highHand, skipping folded and empty pots
        foreach (var player in playersInHand)
        {
            if (player.PotCommitment > 0 && !player.IsFold)
            {
                if (player.HandStrength > highHand)
                {
                    _playerPotModelWinners.Clear();
                    highHand = player.HandStrength;
                    _playerPotModelWinners.Add(player);
                }
                else if (player.HandStrength == highHand)
                {
                    _playerPotModelWinners.Add(player);
                }
            }
        }

        _playerPotModelWinners.Sort((x, y) => x.PotCommitment.CompareTo(y.PotCommitment));
        return _playerPotModelWinners;
    }

    private void PayOutWinners(List<PlayerPotModel> winners, List<PlayerPotModel> playersInHand)
    {
        long collectedSidePot;
        long currentCommitment, collectionAmount;

        var paidWinners = new List<PlayerPotModel>();

        foreach (var playerPot in winners)
        {
            collectedSidePot = 0;
            currentCommitment = playerPot.PotCommitment;
            // Collect from all players who have money in pot
            foreach (var player in playersInHand)
            {
                if (player.PotCommitment > 0)
                {
                    collectionAmount = Math.Min(currentCommitment, player.PotCommitment);
                    player.PotCommitment -= collectionAmount;
                    collectedSidePot += collectionAmount;
                }
            }

            int winnersToPay = 0;
            Console.WriteLine($"winners.count {winners.Count}");

            foreach (var player in winners)
            {
                if (paidWinners.IndexOf(player) == -1)
                {
                    winnersToPay++;
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"collectedSidePot: {collectedSidePot}  winnersToPay: {winnersToPay}");
            Console.ResetColor();

            // Pay unpaid winners, tip dealer with remainders...
            foreach (var player in winners)
            {
                if (paidWinners.IndexOf(player) == -1)
                {
                    player.ChipsRemaining += collectedSidePot / winnersToPay;

                    if (player.PotCommitment <= 0)
                    {
                        paidWinners.Add(player);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Win ----> Player: {player.Guid} chips: {player.ChipsRemaining} paid out.");
                        Console.ResetColor();
                    }
                }
            }
        }

        winners.Clear();
    }

    // Only count potchips for unfolded players. Also prints status to Console.
    private long PotChipsRemaining(List<PlayerPotModel> playersInHand)
    {
        long tally = 0;

        foreach (var player in playersInHand)
        {
            if (player.IsFold)
            {
                ShowInfo(player, ConsoleColor.Cyan);
            }
            else
            {
                ShowInfo(player, ConsoleColor.Blue);
                tally += player.PotCommitment;
            }
        }

        return tally;
    }

    private static void ShowInfo(PlayerPotModel playerPot, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(
            $"Player: {playerPot.Guid}   \tchips: {playerPot.ChipsRemaining}    \tCommitment: {playerPot.PotCommitment}   \tHandStrength: {playerPot.HandStrength}  \tIsNonTurn: {playerPot.IsFold}");
        Console.ResetColor();
    }
}