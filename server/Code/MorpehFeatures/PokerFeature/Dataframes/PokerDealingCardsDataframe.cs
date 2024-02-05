using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.PokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.PokerFeature.Dataframes;

public struct PokerDealingCardsDataframe : INetworkDataframe
{
    public List<PokerCardNetworkModel> Cards;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Cards?.Count ?? 0);

        if (Cards != null)
        {
            foreach (var user in Cards)
            {
                writer.Write(user);
            }
        }
    }
    
    public void Read(NetFrameReader reader)
    {
        var count = reader.ReadInt();

        if (count > 0)
        {
            Cards = new List<PokerCardNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                Cards.Add(reader.Read<PokerCardNetworkModel>());
            }
        }
    }
}