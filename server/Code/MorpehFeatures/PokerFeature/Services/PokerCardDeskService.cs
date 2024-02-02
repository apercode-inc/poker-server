using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.PokerFeature.Enums;
using server.Code.MorpehFeatures.PokerFeature.Models;

namespace server.Code.MorpehFeatures.PokerFeature.Services;

public class PokerCardDeskService : IInitializer
{
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

    public RandomList<CardModel> GetNewCardDeskPokerStandard()
    {
        var newCardDesk = new RandomList<CardModel>(52);

        foreach (var card in _cardDeskPokerStandard)
        {
            newCardDesk.Add(card);
        }
        
        return newCardDesk;
    }
    

    public void Dispose()
    {
        _cardDeskPokerStandard = null;
    }
}