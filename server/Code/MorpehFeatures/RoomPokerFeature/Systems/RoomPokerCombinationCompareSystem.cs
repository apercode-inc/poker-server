using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCombinationCompareSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerShowdown> _roomPokerShowdown;
    [Injectable] private Stash<RoomPokerCombinationMax> _roomPokerCombinationMax; //todo сбросить после победы
    
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination; //todo сбросить после победы
    [Injectable] private Stash<PlayerPokerWin> _playerPokerWin; //todo сбросить послен победы
    
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
            .With<RoomPokerShowdown>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerCombinationMax = ref _roomPokerCombinationMax.Get(roomEntity);
            var combinationMax = roomPokerCombinationMax.CombinationMax;

            _roomPokerCombinationMax.Remove(roomEntity);
            _roomPokerShowdown.Remove(roomEntity); //todo не должен тут сниматься, должен идти дальше в следующую систему вскрытия и победы
            
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

            CompareCombination(combinationMax);
        }
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

        if (combinationType == CombinationType.FourOfKind)
        {
            DefineFourOfKind();
        }
    }

    private void DefineFourOfKind()
    {
        var maxRank = CardRank.Two;
        var kikerRank = CardRank.Two;

        foreach (var playerByCards in _playersByCards)
        {
            var cards = playerByCards.Value;

            var counterKind = 0;
            var rankTemp = CardRank.Two;
            foreach (var card in cards)
            {
                //if (card.Rank )
            }
        }
    }

    private void DefineAllWinnings()
    {
        foreach (var playerByCards in _playersByCards)
        {
            _playerPokerWin.Set(playerByCards.Key);
        }
    }

    private void DefineStraightWinnings()
    {
        var maxRank = CardRank.Two;

        foreach (var playerByCards in _playersByCards)
        {
            var cards = playerByCards.Value;

            var highRank = cards.First().Rank;
            if (maxRank < highRank)
            {
                maxRank = highRank;
            }
        }

        foreach (var playersByCard in _playersByCards)
        {
            var player = playersByCard.Key;
            var cards = playersByCard.Value;

            var highRank = cards.First().Rank;

            if (highRank == maxRank)
            {
                _playerPokerWin.Set(player);
            }
        }
    }

    public void Dispose()
    {
        _playersByCards = null;
        _filter = null;
    }
}