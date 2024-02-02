using server.Code.MorpehFeatures.PokerFeature.Enums;

namespace server.Code.MorpehFeatures.PokerFeature.Models;

public struct CardModel
{
    public CardRank Rank;
    public CardSuit Suit;

    public CardModel(CardRank rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }
}