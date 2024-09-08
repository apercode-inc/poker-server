using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
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

    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;

    private Filter _filter;
    private Dictionary<Entity, List<CardModel>> _playersByCards;

    public World World { get; set; }

    public void OnAwake()
    {
        _playersByCards = new Dictionary<Entity, List<CardModel>>();

        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerSetStrengthHand>()
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
            _roomPokerSetStrengthHand.Remove(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerPokerCombination = ref _playerPokerCombination.Get(player, out var combinationExist);

                if (!combinationExist)
                {
                    continue;
                }

                playerPokerCombination.StrengthHand = GetStrengthCombination(playerPokerCombination.CombinationType,
                    playerPokerCombination.CombinationCards);
                
                //todo test
                ref var playerNickname = ref player.GetComponent<PlayerNickname>();
                Logger.LogWarning($"Player:{playerNickname.Value}, StrengthHand:{playerPokerCombination.StrengthHand}");
            }
        }
    }

    private int GetStrengthCombination(CombinationType combinationStrength, List<CardModel> combinationCards)
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
            case CombinationType.Flush:
            case CombinationType.Straight:
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
    
    private int GetStrengthCombination(List<CardModel> combinationCards)
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
}