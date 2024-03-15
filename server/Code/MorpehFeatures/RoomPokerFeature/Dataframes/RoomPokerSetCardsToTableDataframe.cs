using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSetCardsToTableDataframe : INetworkDataframe
{
    public long Bank;
    public CardToTableState CardToTableState;
    public List<RoomPokerCardNetworkModel> Cards;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(Bank);
        writer.WriteByte((byte) CardToTableState);
        
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
        CardToTableState = (CardToTableState) reader.ReadByte();
        
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