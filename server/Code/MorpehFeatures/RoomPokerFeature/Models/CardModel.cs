using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Models;

public class CardModel
{
    public CardRank Rank;
    public CardSuit Suit;
    public bool IsHands;

    public CardModel(CardRank rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }
}