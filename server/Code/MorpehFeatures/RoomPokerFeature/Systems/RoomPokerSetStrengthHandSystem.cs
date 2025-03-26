using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetStrengthHandSystem : ISystem
{
    private const int CombinationMultiplier = 1000000;
    private const int MultiplierForMultiplier = 10;

    [Injectable] private Stash<RoomPokerSetStrengthHand> _roomPokerSetStrengthHand;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;

    [Injectable] private PlayerStorage _playerStorage;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerSetStrengthHand>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerSetStrengthHand.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.PlayersBySeat)
            {
                if (!markedPlayer.IsOccupied)
                {
                    continue;
                }
                
                var player = markedPlayer.Player;

                ref var playerPokerCombination = ref _playerPokerCombination.Get(player, out var combinationExist);

                if (!combinationExist)
                {
                    continue;
                }

                playerPokerCombination.HandStrength = GetStrengthCombination(playerPokerCombination.CombinationType,
                    ref playerPokerCombination.CombinationCards);

                ref var playerAuthData = ref _playerAuthData.Get(player);

                foreach (var playerPotModel in roomPokerPlayers.PlayerPotModels)
                {
                    if (playerAuthData.Guid != playerPotModel.Guid)
                    {
                        continue;
                    }
                    
                    playerPotModel.SetHandStrength(playerPokerCombination.HandStrength);
                    break;
                }
            }

            _roomPokerPayoutWinnings.Set(roomEntity);
        }
    }

    private int GetStrengthCombination(CombinationType combinationStrength, ref List<CardModel> combinationCards)
    {
        var strength = 0;
        switch (combinationStrength)
        {
            case CombinationType.RoyalFlush:
                break;
            case CombinationType.FullHouse:
            case CombinationType.TwoPair:
            case CombinationType.OnePair:
            case CombinationType.ThreeOfKind:
            case CombinationType.FourOfKind: 
                combinationCards = GetSortCombinationByPriority(combinationCards);
                strength = GetStrengthCombination(combinationCards);
                break;
            case CombinationType.StraightFlush:
            case CombinationType.LowStraightFlush:
            case CombinationType.Flush:
            case CombinationType.Straight:
            case CombinationType.LowStraight:
            case CombinationType.HighCard:
                strength = GetStrengthCombination(combinationCards);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(combinationStrength), combinationStrength, null);
        }

        var totalStrength = CombinationMultiplier * (int)combinationStrength + strength;
        
        return totalStrength;
    }

    private List<CardModel> GetSortCombinationByPriority(IEnumerable<CardModel> cards)
    {
        var groupedCards = cards.GroupBy(card => card.Rank)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => group.Key);
        
        var sortedGroups = groupedCards
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => group.Key);
        
        var sortedCards = sortedGroups
            .SelectMany(group => group)
            .ToList();

        return sortedCards;
    }
    
    private int GetStrengthCombination(IReadOnlyList<CardModel> combinationCards)
    {
        var multiplier = 1;
        var strength = 0;
        
        for (var i = combinationCards.Count - 1; i >= 0; i--)
        {
            var card = combinationCards[i];
            strength += multiplier * (int)card.Rank;

            multiplier *= MultiplierForMultiplier;
        }
        
        return strength;
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}