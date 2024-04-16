using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCombinationCompareSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;
    
    [Injectable] private Stash<RoomPokerCombinationMax> _roomPokerCombinationMax;
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
            .With<RoomPokerCombinationMax>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerCombinationMax = ref _roomPokerCombinationMax.Get(roomEntity);
            var combinationMax = roomPokerCombinationMax.CombinationMax;

            _roomPokerCombinationMax.Remove(roomEntity);
            
            _playersByCards.Clear();

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerPokerCombination = ref _playerPokerCombination.Get(player, out var combinationExist);
                
                if (!combinationExist || playerPokerCombination.CombinationType != combinationMax)
                {
                    continue;
                }
                
                _playersByCards.Add(player, playerPokerCombination.CombinationCards);
            }

            var winningPlayers = new FastList<Entity>();
            
            if (_playersByCards.Count == 1)
            {
                winningPlayers.Add(_playersByCards.First().Key);
            }
            else
            {
                winningPlayers = DefineWinningPlayersByCombination(combinationMax, _playersByCards);
            }
            
            roomEntity.SetComponent(new RoomPokerPlayersGivenBank
            {
                Players = winningPlayers,
            });
        }
    }

    private FastList<T> DefineWinningPlayersByCombination<T>(CombinationType combinationType,
        Dictionary<T, List<CardModel>> playersByCards) where T : class
    {
        FastList<T> winningPlayers = null;

        switch (combinationType)
        {
            case CombinationType.RoyalFlush:
                winningPlayers = DefineAllWinnings(playersByCards);
                break;
            case CombinationType.FourOfKind: 
            case CombinationType.FullHouse:
            case CombinationType.ThreeOfKind:
            case CombinationType.TwoPair:
            case CombinationType.OnePair:
                winningPlayers = DefineWinningPlayersByCombinationSeniority(playersByCards, true);
                break;
            case CombinationType.StraightFlush:
            case CombinationType.Flush:
            case CombinationType.Straight:
            case CombinationType.HighCard:
                winningPlayers = DefineWinningPlayersByCombinationSeniority(playersByCards, false);
                break;
        }

        return winningPlayers;
    }

    private FastList<T> DefineWinningPlayersByCombinationSeniority<T>(Dictionary<T, List<CardModel>> playersByCards, 
        bool isPrioritySort) where T: class
    {
        var winningPlayers = new FastList<T>();
        
        foreach (var playerByCards in playersByCards)
        {
            if (isPrioritySort)
            {
                playersByCards[playerByCards.Key] = GetSortCombinationByPriority(playerByCards.Value);
            }
            
            winningPlayers.Add(playerByCards.Key);
        }
        
        foreach (var playerOne in playersByCards.Keys)
        {
            foreach (var playerTwo in playersByCards.Keys)
            {
                if (playerOne.Equals(playerTwo))
                {
                    continue;
                }

                var playerOneCards = playersByCards[playerOne];
                var playerTwoCards = playersByCards[playerTwo];

                var player1Wins = CompareCards(playerOneCards, playerTwoCards);

                if (!player1Wins)
                {
                    winningPlayers.Remove(playerOne);
                    break;
                }
            }
        }

        return winningPlayers;
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
    
    private bool CompareCards(IReadOnlyList<CardModel> cardsOne, IReadOnlyList<CardModel> cardsTwo)
    {
        for (var i = 0; i < cardsOne.Count; i++)
        {
            var comparisonResult = cardsOne[i].Rank.CompareTo(cardsTwo[i].Rank);
            
            if (comparisonResult != 0)
            {
                return comparisonResult > 0;
            }
        }

        return true;
    }

    private FastList<T> DefineAllWinnings<T>(Dictionary<T, List<CardModel>> playersByCards) where T : class
    {
        var winningPlayers = new FastList<T>();

        foreach (var playersByCard in playersByCards)
        {
            winningPlayers.Add(playersByCard.Key);
        }

        return winningPlayers;
    }

    public void Dispose()
    {
        _playersByCards = null;
        _filter = null;
    }
}