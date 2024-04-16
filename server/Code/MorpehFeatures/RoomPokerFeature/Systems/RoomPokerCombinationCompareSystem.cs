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
            
            //_playersByCards.Clear();
            var playerGivenBank = new FastList<Entity>(); //todo заглушка выигрывают все со старшей комбинацией
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerPokerCombination = ref _playerPokerCombination.Get(player, out var combinationExist);
                
                if (!combinationExist || playerPokerCombination.CombinationType != combinationMax)
                {
                    continue;
                }
                
                playerGivenBank.Add(player); //todo заглушка выигрывают все со старшей комбинацией
                //_playersByCards.Add(player, playerPokerCombination.CombinationCards);
            }
            
            roomEntity.SetComponent(new RoomPokerPlayersGivenBank
            {
                Players = playerGivenBank,
            });

            //CompareCombination(combinationMax);
        }
    }
    
    public void SortCombinationAndKickers(List<CardModel> cards) //Использовать для: FourOfKind, ThreeOfKind, TwoPair, OnePair
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
        
        Logger.Debug($"Сортировка (комбинации, кикеры) === {Logger.GetCardsLog(sortedCards)}", ConsoleColor.DarkBlue);
    }

    private void CompareCombination(CombinationType combinationType)
    {
        if (combinationType == CombinationType.RoyalFlush)
        {
            DefineAllWinnings();
        }

        if (combinationType == CombinationType.StraightFlush)
        {
            DefineStraightWinnings();
        }

        if (combinationType == CombinationType.FourOfKind) //todo 1 кикер
        {
            DefineFourOfKindWinnings();
        }

        if (combinationType == CombinationType.FullHouse)
        {
            DefineFullHouseWinnings();
        }

        if (combinationType == CombinationType.Flush)
        {
            //тут по идеи тоже самое что и стрит
            ////нет не тоже самое вдруг первая карта будет одинаковой, значит надо сравнивать следующую и т.д
            //DefineStraightWinnings(); 
        }
        
        if (combinationType == CombinationType.Straight)
        {
            DefineStraightWinnings();
        }

        if (combinationType == CombinationType.ThreeOfKind) //todo 2 кикера
        {
            DefineThreeOfKindWinnings();
        }
        
        if (combinationType == CombinationType.TwoPair) //todo 1 кикер
        {
            DefineTwoPairWinnings();
        }
        
        if (combinationType == CombinationType.OnePair) //todo 3 кикера
        {
            DefineOnePairWinnings();
        }
        
        if (combinationType == CombinationType.HighCard) //todo 4 кикера
        {
            DefineHighCardWinnings();
        }
    }

    private void DefineHighCardWinnings()
    {
        
    }

    private void DefineOnePairWinnings()
    {
       
    }

    private void DefineTwoPairWinnings()
    {
        
    }

    private void DefineThreeOfKindWinnings()
    {
        
    }

    private void DefineFullHouseWinnings()
    {
        
    }

    private void DefineFourOfKindWinnings()
    {
        foreach (var playerByCards in _playersByCards) //todo подумать пока что хуйня выходит
        {
            var cards = playerByCards.Value;
            
            var maxKikerRank = CardRank.Two;
            var maxRankFourOfKind = cards[2].Rank;
            
            if (cards[0].Rank != maxRankFourOfKind) //A-5-5-5-5 
            {
                maxKikerRank = cards[0].Rank;
            }
            
            if (cards[4].Rank != maxRankFourOfKind) //A-A-A-A-5
            {
                maxKikerRank = cards[4].Rank;
            }
        }
    }

    private void DefineAllWinnings()
    {
        foreach (var playerByCards in _playersByCards)
        {
            //_playerPokerWin.Set(playerByCards.Key);
        }
    }

    private void DefineStraightWinnings()
    {
        var maxRank = CardRank.Two;

        foreach (var playerByCards in _playersByCards)
        {
            var cards = playerByCards.Value;

            var highRank = cards[0].Rank;
            if (maxRank < highRank)
            {
                maxRank = highRank;
            }
        }

        foreach (var playersByCard in _playersByCards)
        {
            var player = playersByCard.Key;
            var cards = playersByCard.Value;

            var highRank = cards[0].Rank;

            if (highRank == maxRank)
            {
                //_playerPokerWin.Set(player);
            }
        }
    }

    public void Dispose()
    {
        _playersByCards = null;
        _filter = null;
    }
}