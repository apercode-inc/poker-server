using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSetCardsByPlayerDataframe : INetworkDataframe
{
    public int PlayerId;
    public CardsState CardsState;
    public List<RoomPokerCardNetworkModel> Cards;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteInt((int) CardsState);
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
        PlayerId = reader.ReadInt();
        CardsState = (CardsState) reader.ReadInt();    
        
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