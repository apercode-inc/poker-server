using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.ShowCombinationFeature.Dataframes;

public struct ShowCombinationWinDataframe : INetworkDataframe
{
    public int PlayerId;
    public CombinationType CombinationType;
    public List<RoomPokerCardNetworkModel> Cards;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteShort((short) CombinationType);

        var haCards = Cards != null;
        writer.WriteBool(haCards);

        if (haCards)
        {
            writer.WriteInt(Cards.Count);

            foreach (var card in Cards)
            {
                writer.Write(card);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        CombinationType = (CombinationType)reader.ReadShort();
        
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