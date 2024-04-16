using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
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
    
    [Injectable] private Stash<RoomPokerCombinationMax> _roomPokerCombinationMax; //todo сбросить после победы
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination; //todo сбросить после победы
    
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

    public FastList<T> DefineWinningPlayersByCombination<T>(CombinationType combinationType,
        Dictionary<T, List<CardModel>> playersByCards) where T: class
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
                winningPlayers = DefineWinningPlayersByCardsSeniority(playersByCards, true);
                break;
            case CombinationType.StraightFlush:
            case CombinationType.Flush:
            case CombinationType.Straight:
            case CombinationType.HighCard:
                winningPlayers = DefineWinningPlayersByCardsSeniority(playersByCards, false);
                break;
        }

        return winningPlayers;
    }

    private FastList<T> DefineWinningPlayersByCardsSeniority<T>(Dictionary<T, List<CardModel>> playersByCards, bool kickersSort) where T: class
    {
        var winningPlayers = new FastList<T>();
        
        foreach (var playerByCards in playersByCards)
        {
            if (kickersSort)
            {
                playersByCards[playerByCards.Key] = GetSortCombinationAndKickers(playerByCards.Value);
            }
            
            winningPlayers.Add(playerByCards.Key);
        }
        
        // Сравниваем карты каждого игрока с картами других игроков
        foreach (var player1 in playersByCards.Keys)
        {
            foreach (var player2 in playersByCards.Keys)
            {
                if (player1.Equals(player2))
                {
                    continue;
                }

                var player1Cards = playersByCards[player1];
                var player2Cards = playersByCards[player2];

                var player1Wins = CompareCards(player1Cards, player2Cards);

                if (!player1Wins)
                {
                    winningPlayers.Remove(player1);
                    break;
                }
            }
        }

        return winningPlayers;
    }
    
    private List<CardModel> GetSortCombinationAndKickers(List<CardModel> cards)
    {
        var groupedCards = cards.GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key);
        
        var sortedGroups = groupedCards
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key);
        
        var sortedCards = sortedGroups
            .SelectMany(g => g)
            .ToList();
        
        Logger.Debug($"Сортировка (комбинации, кикеры) === {Logger.GetCardsLog(cards)}", ConsoleColor.DarkBlue);
        
        return sortedCards;
    }
    
    private bool CompareCards(IReadOnlyList<CardModel> cards1, IReadOnlyList<CardModel> cards2)
    {
        // Последовательно сравниваем карты каждого игрока
        for (int i = 0; i < cards1.Count; i++)
        {
            int comparisonResult = cards1[i].Rank.CompareTo(cards2[i].Rank); // Сравниваем ранги карт
            if (comparisonResult != 0)
            {
                return comparisonResult > 0; // Возвращаем true, если карта игрока 1 лучше, иначе false
            }
        }

        return true; //TODO так не должно быть (Если все карты игрока 1 равны картам игрока 2, игрок 1 побеждает)
    }

    private FastList<T> DefineAllWinnings<T>(Dictionary<T, List<CardModel>> playersByCards) where T: class
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