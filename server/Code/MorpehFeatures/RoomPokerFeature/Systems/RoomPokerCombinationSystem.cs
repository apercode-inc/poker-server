using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

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

                var combination = DetermineCombination(playerCards.Cards, roomPokerCardsToTable.Cards);

                ref var playerNickname = ref _playerNickname.Get(player);
                
                Debug.LogColor($"player: {playerNickname.Value} combination : {combination}", ConsoleColor.Magenta);

                // _playerCombination.Set(player, new PlayerCombination
                // {
                //     CombinationType = CombinationType.HighCard,
                //     Card = cardHigh,
                // });
            }
        }
    }
    
    public CombinationType DetermineCombination(IEnumerable<CardModel> playerCards, IEnumerable<CardModel> tableCards)
    {
        var allCards = new List<CardModel>(playerCards);
        allCards.AddRange(tableCards);

        allCards.Sort((x, y) => y.Rank.CompareTo(x.Rank)); // Сортировка карт по убыванию ранга

        if (IsRoyalFlush(allCards))
        {
            return CombinationType.RoyalFlush;
        }

        if (IsStraightFlush(allCards))
        {
            return CombinationType.StraightFlush;
        }

        if (IsFourOfAKind(allCards))
        {
            return CombinationType.FourKing;
        }

        if (IsFullHouse(allCards))
        {
            return CombinationType.FullHouse;
        }

        if (IsFlush(allCards))
        {
            return CombinationType.Flush;
        }

        if (IsStraight(allCards))
        {
            return CombinationType.Straight;
        }

        if (IsThreeOfAKind(allCards))
        {
            return CombinationType.ThreeKing;
        }

        if (IsTwoPair(allCards))
        {
            return CombinationType.TwoPair;
        }

        if (IsPair(allCards))
        {
            return CombinationType.OnePair;
        }

        return CombinationType.HighCard;
    }
    
    private bool IsRoyalFlush(List<CardModel> cards)
    {
        return IsStraightFlush(cards) && cards[0].Rank == CardRank.Ace;
    }

    private bool IsStraightFlush(List<CardModel> cards)
    {
        var suitsCount = new int[4];
        foreach (var card in cards)
        {
            suitsCount[(int)card.Suit]++;
        }
    
        for (var i = 0; i < cards.Count - 4; i++)
        {
            var isStraightFlush = true;
            for (var j = 0; j < 4; j++)
            {
                if (cards[i + j].Rank - 1 != cards[i + j + 1].Rank || cards[i + j].Suit != cards[i + j + 1].Suit)
                {
                    isStraightFlush = false;
                    break;
                }
            }
    
            if (isStraightFlush)
            {
                return true;
            }
        }
    
        return false;
    }


    private bool IsFourOfAKind(List<CardModel> cards)
    {
        for (int i = 0; i <= cards.Count - 4; i++)
        {
            if (cards[i].Rank == cards[i + 3].Rank)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsFullHouse(List<CardModel> cards)
    {
        var threeOfAKindFound = false;
        var pairFound = false;

        for (var i = 0; i <= cards.Count - 3; i++)
        {
            if (cards[i].Rank != cards[i + 2].Rank)
            {
                continue;
            }
            
            threeOfAKindFound = true;

            for (var j = 0; j <= cards.Count - 2; j++)
            {
                if (j == i || j == i + 1 || j == i + 2 || cards[j].Rank != cards[j + 1].Rank)
                {
                    continue;
                }
                    
                pairFound = true;
                break;
            }

            if (pairFound)
            {
                break;
            }
        }

        return threeOfAKindFound && pairFound;
    }

    
    private bool IsFlush(List<CardModel> cards)
    {
        var suitsCount = new int[4];
        foreach (var card in cards)
        {
            suitsCount[(int)card.Suit]++;
        }

        return suitsCount.Any(count => count >= 5);
    }

    private bool IsStraight(List<CardModel> cards)
    {
        for (var i = 0; i < cards.Count - 4; i++)
        {
            var isStraight = true;
            for (var j = 0; j < 4; j++)
            {
                if (cards[i + j].Rank - 1 != cards[i + j + 1].Rank)
                {
                    isStraight = false;
                    break;
                }
            }

            if (isStraight)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsThreeOfAKind(List<CardModel> cards)
    {
        for (var i = 0; i <= cards.Count - 3; i++)
        {
            if (cards[i].Rank == cards[i + 2].Rank)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTwoPair(List<CardModel> cards)
    {
        var pairsCount = 0;
        for (var i = 0; i <= cards.Count - 2; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank)
            {
                pairsCount++;
                i++;
            }
        }

        return pairsCount >= 2;
    }

    private bool IsPair(List<CardModel> cards)
    {
        for (var i = 0; i <= cards.Count - 2; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank)
            {
                return true;
            }
        }

        return false;
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}