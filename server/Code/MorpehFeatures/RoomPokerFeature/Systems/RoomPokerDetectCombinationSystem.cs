using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Utils;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDetectCombinationSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;
    [Injectable] private Stash<RoomPokerDetectCombination> _roomPokerDetectCombination;
    [Injectable] private Stash<RoomPokerSetStrengthHand> _roomPokerSetStrengthHand;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerDetectCombination>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);

            var combinationMax = CombinationType.HighCard;
            
            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerCards = ref _playerCards.Get(player);

                if (playerCards.CardsState == CardsState.Empty)
                {
                    continue;
                }
                
                var combination = GetPokerCombination(playerCards.Cards, roomPokerCardsToTable.Cards, 
                    out var combinationOrderedCards);

                _playerPokerCombination.Set(player, new PlayerPokerCombination
                {
                    CombinationType = combination,
                    CombinationCards = combinationOrderedCards,
                });
            }
            
            _roomPokerSetStrengthHand.Set(roomEntity);
            _roomPokerDetectCombination.Remove(roomEntity);
        }
    }

    private CombinationType GetPokerCombination(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards, 
        out List<CardModel> combinationOrderedCards)
    {
        var allCards = new List<CardModel>(playerCards);
        
        allCards.AddRange(tableCards);
        allCards.Sort((x, y) => y.Rank.CompareTo(x.Rank));
        
        var subsetsForSeat = CombinationUtils.CombinationsRosettaWoRecursion(allCards.ToArray(), 5);
        
        CombinationType combinationTypeMax = default;

        var forSeat = subsetsForSeat as List<CardModel>[] ?? subsetsForSeat.ToArray();
        combinationOrderedCards = forSeat.First();
        
        foreach (var subset in forSeat)
        {
            var combinationType = DetectPokerCombination(subset);
            
            if (combinationType <= combinationTypeMax)
            {
                continue;
            }
            combinationTypeMax = combinationType;
            combinationOrderedCards = subset;
        }

        return combinationTypeMax;
    }

    private CombinationType DetectPokerCombination(List<CardModel> cards)
    {
        ulong handValue = 0;

        var cardValues = new Dictionary<int, int>();
        foreach (var card in cards)
        {
            if (cardValues.ContainsKey((int)card.Rank))
            {
                cardValues[(int)card.Rank]++;
            }
            else
            {
                cardValues[(int)card.Rank] = 1;
            }
        }

        foreach (var cardValue in cardValues.Keys)
        {
            var cardsCount = cardValues[cardValue];
            for (var i = 0; i < cardsCount; i++)
            {
                handValue += (ulong)Math.Pow(2, i) << cardValue * 4;
            }
        }
        
        var normalizedValue = handValue % 15;
        switch (normalizedValue)
        {
            case 1:
                return CombinationType.FourOfKind;
            case 10:
                return CombinationType.FullHouse;
            case 9:
                return CombinationType.ThreeOfKind;
            case 7:
                return CombinationType.TwoPair;
            case 6:
                return CombinationType.OnePair;
        }
        
        var isSameSuit = cards.TrueForAll(card => card.Suit == cards[0].Suit);
        var isOrdered = true;
        CardRank? tmpValue = null;
        
        var lowOrderCounter = 0;
        foreach (var card in cards.OrderBy(card => card.Rank))
        {
            if (card.Rank is CardRank.Ace or CardRank.Two or CardRank.Three or CardRank.Four or CardRank.Five)
            {
                lowOrderCounter++;
            }
            
            if (tmpValue.HasValue)
            {
                if (card.Rank - tmpValue.Value != 1)
                {
                    isOrdered = false;
                    break;
                }

                tmpValue = card.Rank;
            }
            else
            {
                tmpValue = card.Rank;
            }
        }

        if (!isOrdered && lowOrderCounter == cards.Count)
        {
            var aceCard = cards.First();
            cards.Remove(aceCard);
            cards.Add(aceCard);
            
            isOrdered = true;
        }
        
        var isHighAce = tmpValue == CardRank.Ace;

        if (!isSameSuit)
        {
            return isOrdered ? CombinationType.Straight : CombinationType.HighCard;
        }
        
        if (isOrdered)
        {
            return isHighAce ? CombinationType.RoyalFlush : CombinationType.StraightFlush;
        }

        return CombinationType.Flush;
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}