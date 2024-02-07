using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerDealingCardsDataframe : INetworkDataframe
{
    public List<RoomPokerCardNetworkModel> CardsForLocal;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(CardsForLocal?.Count ?? 0);

        if (CardsForLocal != null)
        {
            foreach (var user in CardsForLocal)
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
            CardsForLocal = new List<RoomPokerCardNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                CardsForLocal.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
    }
}