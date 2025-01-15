using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCalculatePayoutWinningsSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerPaidOutToPlayers> _roomPokerPaidOutToPlayers;

    [Injectable] private ConfigsService _configsService;
    
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

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            var players = roomPokerPlayers.PlayerPotModels;

            var paidOutToPlayers = new List<List<PlayerPotModel>>();
            
            while (PotChipsRemaining(players) > 0)
            {
                var payOutWinners = GetPayOutWinners(CalculateAndSortWinners(players), players);
                paidOutToPlayers.Add(payOutWinners);
            }

            // Refund players if remaining chips in pot (bigger/folded stacks)
            paidOutToPlayers.Add(GetPayOutRefund(players));
            
            var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
            var delayPayOut = config.DelayPayOut;
            
            _roomPokerPaidOutToPlayers.Set(roomEntity, new RoomPokerPaidOutToPlayers
            {
                PaidDelay = delayPayOut,
                PaidCooldown = 0,
                PaidOutToPlayers = paidOutToPlayers,
            });

            _roomPokerPayoutWinnings.Remove(roomEntity);
        }
    }

    private List<PlayerPotModel> CalculateAndSortWinners(List<PlayerPotModel> playersInHand)
    {
        var highHand = 0;
        
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

    private List<PlayerPotModel> GetPayOutWinners(List<PlayerPotModel> winners, List<PlayerPotModel> playersInHand)
    {
        var paidWinners = new List<PlayerPotModel>();

        foreach (var playerPot in winners)
        {
            long collectedSidePot = 0;
            var currentCommitment = playerPot.PotCommitment;
            // Collect from all players who have money in pot
            foreach (var player in playersInHand)
            {
                if (player.PotCommitment > 0)
                {
                    var collectionAmount = Math.Min(currentCommitment, player.PotCommitment);
                    player.PotCommitment -= collectionAmount;
                    collectedSidePot += collectionAmount;
                }
            }

            var winnersToPay = 0;

            foreach (var player in winners)
            {
                if (paidWinners.IndexOf(player) == -1)
                {
                    winnersToPay++;
                }
            }

            // Pay unpaid winners, tip dealer with remainders...
            foreach (var player in winners)
            {
                if (paidWinners.IndexOf(player) == -1)
                {
                    player.ChipsRemaining += collectedSidePot / winnersToPay;

                    if (player.PotCommitment <= 0)
                    {
                        paidWinners.Add(player);
                    }
                }
            }
        }

        winners.Clear();
        return paidWinners;
    }
    
    private List<PlayerPotModel> GetPayOutRefund(List<PlayerPotModel> players)
    {
        var paidRefund = new List<PlayerPotModel>();
        
        foreach (var player in players)
        {
            if (!player.IsFold || player.PotCommitment == 0)
            {
                continue;
            }

            player.ChipsRemaining += player.PotCommitment;
            player.PotCommitment = 0;
            
            paidRefund.Add(player);
        }
        
        return paidRefund;
    }

    // Only count potchips for unfolded players.
    private long PotChipsRemaining(List<PlayerPotModel> playersInHand)
    {
        long total = 0;

        foreach (var player in playersInHand)
        {
            if (!player.IsFold)
            {
                total += player.PotCommitment;
            }
        }

        return total;
    }
    
    public void Dispose()
    {
        _playerPotModelWinners = null;
        _filter = null;
    }
}