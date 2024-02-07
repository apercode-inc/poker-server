using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPokerCardNetworkModel : IWriteable, IReadable
{
    public CardRank Rank;
    public CardSuit Suit;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt((int) Rank);
        writer.WriteInt((int) Suit);
    }

    public void Read(NetFrameReader reader)
    {
        Rank = (CardRank) reader.ReadInt();
        Suit = (CardSuit)reader.ReadInt();
    }
}