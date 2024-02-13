using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerDealingCardsDataframe : INetworkDataframe
{
    public List<RoomPokerCardNetworkModel> Cards;

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
            Cards = new List<RoomPokerCardNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                Cards.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
    }
}