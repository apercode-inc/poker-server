using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPokerShowdownNetworkModel : IWriteable, IReadable
{
    public int PlayerId;
    public List<RoomPokerCardNetworkModel> Cards;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        
        var hasCards = Cards != null;
        writer.WriteBool(hasCards);

        if (hasCards)
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
        PlayerId = reader.ReadInt();
        
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