using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSetCardsToTableDataframe : INetworkDataframe
{
    public long Bank;
    public List<RoomPokerCardNetworkModel> Cards;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(Bank);
        
        var hasUsers = Cards != null;
        writer.WriteBool(hasUsers);

        if (hasUsers)
        {
            writer.WriteInt(Cards.Count);

            foreach (var user in Cards)
            {
                writer.Write(user);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        Bank = reader.ReadLong();
        
        if (reader.ReadBool())
        {
            var count = reader.ReadInt();
            Cards = new List<RoomPokerCardNetworkModel>();

            for (var i = 0; i < count; i++)
            {
                Cards.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
    }
}