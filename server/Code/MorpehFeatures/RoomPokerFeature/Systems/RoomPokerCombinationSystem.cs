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

                var combination = DetermineCombination(playerCards.Cards, roomPokerCardsToTable.Cards, 
                    out var kickerRanks, out var advantageRank);

                ref var playerNickname = ref _playerNickname.Get(player);

                Debug.LogColor(new string('-', 50), ConsoleColor.Yellow);
                Debug.LogColor($"player: {playerNickname.Value} combination : {combination}", ConsoleColor.Magenta);

                foreach (var kicker in kickerRanks)
                {
                    Debug.LogColor($"kicker: {kicker}", ConsoleColor.Magenta);
                }
                Debug.LogColor(new string('-', 50), ConsoleColor.Yellow);

                // _playerCombination.Set(player, new PlayerCombination
                // {
                //     CombinationType = CombinationType.HighCard,
                //     Card = cardHigh,
                // });
            }
        }
    }

    //todo сгенерировано с помощью chat GPT, потестить и разобраться с кикерами
    public CombinationType DetermineCombination(IEnumerable<CardModel> playerCards, 
        IEnumerable<CardModel> tableCards, out List<CardRank> kickerRanks, out CardRank advantageRank)
    {
        var allCards = new List<CardModel>(playerCards);
        allCards.AddRange(tableCards);

        // Сортировка карт по убыванию ранга
        allCards.Sort((x, y) => y.Rank.CompareTo(x.Rank));

        var combinationType = CombinationType.HighCard;
        var findKickerRanks = new List<CardRank>();
        
        if (IsStraightFlush(allCards, out advantageRank))
        {
            combinationType = advantageRank == CardRank.Ace ? CombinationType.RoyalFlush : CombinationType.StraightFlush;
        }
        else if (IsFourOfAKind(allCards, out advantageRank))
        {
            combinationType = CombinationType.FourKing;
            findKickerRanks.Add(allCards[0].Rank);
        }
        else if (IsFullHouse(allCards, out advantageRank)) //Достоинство фул-хауса определяется в первую очередь тройкой, а во вторую – парой.
        {
            combinationType = CombinationType.FullHouse;
        }
        else if (IsFlush(allCards, out advantageRank))
        {
            combinationType = CombinationType.Flush;
            findKickerRanks.AddRange(allCards.Select(c => c.Rank).Take(5));
        }
        else if (IsStraight(allCards, out advantageRank))
        {
            combinationType = CombinationType.Straight;
        }
        else if (IsThreeOfAKind(allCards, out advantageRank))
        {
            combinationType = CombinationType.ThreeKing;
            findKickerRanks.AddRange(allCards.Where(c => c.Rank != allCards[2].Rank).Select(c => c.Rank).Take(2));
        }
        else if (IsTwoPair(allCards, out advantageRank))
        {
            combinationType = CombinationType.TwoPair;
            findKickerRanks.AddRange(allCards.Where(c => c.Rank != allCards[2].Rank && c.Rank != allCards[4].Rank)
                .Select(c => c.Rank).Take(1));
        }
        else if (IsPair(allCards, out advantageRank))
        {
            combinationType = CombinationType.OnePair;
            findKickerRanks.AddRange(allCards.Where(c => c.Rank != allCards[2].Rank).Select(c => c.Rank).Take(3));
        }

        kickerRanks = findKickerRanks;
        return combinationType;
    }

    private bool IsStraightFlush(List<CardModel> cards, out CardRank advantageRank)
    {
        var cardsBySuit = cards.GroupBy(c => c.Suit);
        
        foreach (var suitGroup in cardsBySuit)
        {
            var suitCards = suitGroup.ToList();
            
            var hasAce = suitCards.Any(c => c.Rank == CardRank.Ace);
            
            if (hasAce && suitCards.Count >= 5)
            {
                suitCards.Sort((a, b) => a.Rank.CompareTo(b.Rank));
                if (suitCards[0].Rank == CardRank.Two 
                    && suitCards[1].Rank == CardRank.Three 
                    && suitCards[2].Rank == CardRank.Four 
                    && suitCards[3].Rank == CardRank.Five)
                {
                    advantageRank = CardRank.Five;
                    return true;
                }
            }
            
            var advantageRankFind = CardRank.Two;
            
            for (var i = 0; i < suitCards.Count - 4; i++)
            {
                var isStraightFlush = true;
                for (var j = 0; j < 4; j++)
                {
                    if (advantageRankFind < suitCards[i + j].Rank)
                    {
                        advantageRankFind = suitCards[i + j].Rank;
                    }
                    
                    if (suitCards[i + j].Rank - 1 != suitCards[i + j + 1].Rank)
                    {
                        isStraightFlush = false;
                        break;
                    }
                }

                if (isStraightFlush)
                {
                    advantageRank = advantageRankFind;
                    return true;
                }
            }
        }

        advantageRank = default;
        return false;
    }


    private bool IsFourOfAKind(List<CardModel> cards, out CardRank advantageRank)
    {
        for (int i = 0; i <= cards.Count - 4; i++)
        {
            if (cards[i].Rank == cards[i + 3].Rank)
            {
                advantageRank = cards[i].Rank;
                return true;
            }
        }

        advantageRank = default;
        return false;
    }

    private bool IsFullHouse(List<CardModel> cards, out CardRank advantageRank)
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

        advantageRank = default; //todo 
        return threeOfAKindFound && pairFound;
    }

    private bool IsFlush(List<CardModel> cards, out CardRank advantageRank)
    {
        var suitsCount = new int[4];
        foreach (var card in cards)
        {
            suitsCount[(int)card.Suit]++;
        }

        advantageRank = default; //todo
        return suitsCount.Any(count => count >= 5);
    }

    private bool IsStraight(List<CardModel> cards, out CardRank advantageRank)
    {
        // Проверяем, есть ли туз
        var hasAce = cards.Any(c => c.Rank == CardRank.Ace);

        // Проверяем наличие стрита с тузом как 1
        if (hasAce && cards[5].Rank == CardRank.Two 
                   && cards[4].Rank == CardRank.Three 
                   && cards[3].Rank == CardRank.Four 
                   && cards[2].Rank == CardRank.Five)
        {
            advantageRank = CardRank.Five;
            return true;
        }

        // Проверяем наличие обычного стрита
        var advantageRankFind = CardRank.Two;
        
        for (var i = 0; i < cards.Count - 4; i++)
        {
            var isStraight = true;
            for (var j = 0; j < 4; j++)
            {
                if (advantageRankFind < cards[i + j].Rank)
                {
                    advantageRankFind = cards[i + j].Rank;
                }
                
                if (cards[i + j].Rank - 1 != cards[i + j + 1].Rank)
                {
                    isStraight = false;
                    break;
                }
            }

            if (isStraight)
            {
                advantageRank = advantageRankFind;
                return true;
            }
        }

        advantageRank = default;
        return false;
    }

    private bool IsThreeOfAKind(List<CardModel> cards, out CardRank advantageRank)
    {
        for (var i = 0; i <= cards.Count - 3; i++)
        {
            if (cards[i].Rank == cards[i + 2].Rank)
            {
                advantageRank = default; //todo 
                return true;
            }
        }

        advantageRank = default;
        return false;
    }

    private bool IsTwoPair(List<CardModel> cards, out CardRank advantageRank)
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

        advantageRank = default; //todo
        return pairsCount >= 2;
    }

    private bool IsPair(List<CardModel> cards, out CardRank advantageRank)
    {
        for (var i = 0; i <= cards.Count - 2; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank)
            {
                advantageRank = default; //todo
                return true;
            }
        }

        advantageRank = default; //todo
        return false;
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}