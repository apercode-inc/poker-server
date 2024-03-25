using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Utils;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCombinationSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerCombination> _playerCombination;
    [Injectable] private Stash<PlayerNickname> _playerNickname; //todo test

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerShowdown>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerCards = ref _playerCards.Get(player);

                if (playerCards.CardsState == CardsState.Empty)
                {
                    continue;
                }
            }
        }
    }
    
    public void TestMockData(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards)
    {
        var allCards = new List<CardModel>(playerCards);
        
        allCards.AddRange(tableCards);
        allCards.Sort((x, y) => y.Rank.CompareTo(x.Rank));
        
        var subsetsForSeat = CollectionsUtils.CombinationsRosettaWoRecursion(allCards.ToArray(), 5);
        
        CombinationType combinationMaxType = default;

        var forSeat = subsetsForSeat as List<CardModel>[] ?? subsetsForSeat.ToArray();
        var cardsModel = forSeat.First();
        
        foreach (var subset in forSeat)
        {
            var combinationType = DetectCombination(subset);
            if (combinationType > combinationMaxType)
            {
                combinationMaxType = combinationType;
                cardsModel = subset;
            }
        }

        foreach (var cards in cardsModel)
        {
            Debug.LogColor($"Rank: {cards.Rank} | Suit: {cards.Suit} | IsHands: {cards.IsHands}", ConsoleColor.Cyan);
        }
        
        Debug.LogColor($"Combination: {combinationMaxType}", ConsoleColor.Blue);
    }

    private static CombinationType DetectCombination(List<CardModel> cards)
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
                return CombinationType.ThreeOfKing;
            case 7:
                return CombinationType.TwoPair;
            case 6:
                return CombinationType.OnePair;
        }
        
        bool isSameSuit = cards.TrueForAll(card => card.Suit == cards[0].Suit);
        bool isOrdered = true;
        CardRank? tmpValue = null;
        
        foreach (var cardValue in cards.OrderBy(card => card.Rank))
        {
            if (tmpValue.HasValue)
            {
                if (cardValue.Rank - tmpValue.Value != 1)
                {
                    isOrdered = false;
                    break;
                }
                else
                {
                    tmpValue = cardValue.Rank;
                }
            }
            else
            {
                tmpValue = cardValue.Rank;
            }
        }
        
        //Debug.LogError($"tmpValue = {tmpValue}");
        //Debug.LogError($"isOrdered {isOrdered}");
        
        var isHighAce = CardRank.Ace == tmpValue;
        
        if (isSameSuit)
        {
            if (isOrdered)
            {
                return isHighAce ? CombinationType.RoyalFlush : CombinationType.StraightFlush;
            }

            return CombinationType.Flush;
        }
        else
        {
            if (isOrdered)
            {
                return CombinationType.Straight;
            }
            else
            {
                return CombinationType.HighCard;
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}