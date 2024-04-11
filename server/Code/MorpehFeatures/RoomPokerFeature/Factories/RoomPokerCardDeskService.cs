using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Factories;

public class RoomPokerCardDeskService : IInitializer
{
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;
    
    public World World { get; set; }

    private RandomList<CardModel> _cardDeskPokerStandard;

    public void OnAwake()
    {
        _cardDeskPokerStandard = new RandomList<CardModel>(52)
        {
            new(CardRank.Two, CardSuit.Spades),
            new(CardRank.Three, CardSuit.Spades),
            new(CardRank.Four, CardSuit.Spades),
            new(CardRank.Five, CardSuit.Spades),
            new(CardRank.Six, CardSuit.Spades),
            new(CardRank.Seven, CardSuit.Spades),
            new(CardRank.Eight, CardSuit.Spades),
            new(CardRank.Nine, CardSuit.Spades),
            new(CardRank.Ten, CardSuit.Spades),
            new(CardRank.Jack, CardSuit.Spades),
            new(CardRank.Queen, CardSuit.Spades),
            new(CardRank.King, CardSuit.Spades),
            new(CardRank.Ace, CardSuit.Spades),
            
            new(CardRank.Two, CardSuit.Hearts),
            new(CardRank.Three, CardSuit.Hearts),
            new(CardRank.Four, CardSuit.Hearts),
            new(CardRank.Five, CardSuit.Hearts),
            new(CardRank.Six, CardSuit.Hearts),
            new(CardRank.Seven, CardSuit.Hearts),
            new(CardRank.Eight, CardSuit.Hearts),
            new(CardRank.Nine, CardSuit.Hearts),
            new(CardRank.Ten, CardSuit.Hearts),
            new(CardRank.Jack, CardSuit.Hearts),
            new(CardRank.Queen, CardSuit.Hearts),
            new(CardRank.King, CardSuit.Hearts),
            new(CardRank.Ace, CardSuit.Hearts),
            
            new(CardRank.Two, CardSuit.Diamonds),
            new(CardRank.Three, CardSuit.Diamonds),
            new(CardRank.Four, CardSuit.Diamonds),
            new(CardRank.Five, CardSuit.Diamonds),
            new(CardRank.Six, CardSuit.Diamonds),
            new(CardRank.Seven, CardSuit.Diamonds),
            new(CardRank.Eight, CardSuit.Diamonds),
            new(CardRank.Nine, CardSuit.Diamonds),
            new(CardRank.Ten, CardSuit.Diamonds),
            new(CardRank.Jack, CardSuit.Diamonds),
            new(CardRank.Queen, CardSuit.Diamonds),
            new(CardRank.King, CardSuit.Diamonds),
            new(CardRank.Ace, CardSuit.Diamonds),
            
            new(CardRank.Two, CardSuit.Clubs),
            new(CardRank.Three, CardSuit.Clubs),
            new(CardRank.Four, CardSuit.Clubs),
            new(CardRank.Five, CardSuit.Clubs),
            new(CardRank.Six, CardSuit.Clubs),
            new(CardRank.Seven, CardSuit.Clubs),
            new(CardRank.Eight, CardSuit.Clubs),
            new(CardRank.Nine, CardSuit.Clubs),
            new(CardRank.Ten, CardSuit.Clubs),
            new(CardRank.Jack, CardSuit.Clubs),
            new(CardRank.Queen, CardSuit.Clubs),
            new(CardRank.King, CardSuit.Clubs),
            new(CardRank.Ace, CardSuit.Clubs),
        };
    }

    public RandomList<CardModel> CreateCardDeskPokerStandard()
    {
        var newCardDesk = new RandomList<CardModel>(52);

        foreach (var card in _cardDeskPokerStandard)
        {
            newCardDesk.Add(card);
        }
        
        return newCardDesk;
    }
    
    public void ReturnCardsInDeskToPlayer(Entity roomEntity, Entity playerLeft)
    {
        ref var playerCards = ref _playerCards.Get(playerLeft);

        if (playerCards.CardsState == CardsState.Empty)
        {
            return;
        }

        ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

        while (playerCards.Cards.Count > 0)
        {
            var card = playerCards.Cards.Dequeue();
            card.IsHands = false;
            roomPokerCardDesk.CardDesk.Add(card);
        }

        playerCards.CardsState = CardsState.Empty;
    }

    public void ReturnCardsInDeskToTable(Entity roomEntity)
    {
        ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);
        ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

        while (roomPokerCardsToTable.Cards.Count > 0)
        {
            var card = roomPokerCardsToTable.Cards.Dequeue();
            roomPokerCardDesk.CardDesk.Add(card);
        }

        roomPokerCardsToTable.State = CardToTableState.PreFlop;
    }
    
    public void Dispose()
    {
        _cardDeskPokerStandard = null;
    }
}