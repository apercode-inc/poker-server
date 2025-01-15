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

        var count = Cards?.Count ?? 0;
        writer.WriteInt(count);

        if (count != 0)
        {
            foreach (var card in Cards)
            {
                writer.Write(card);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        CardsState = (CardsState) reader.ReadInt();    
            
        Cards = null; //todo временный костыль, нужно исправлять в NetFrame. Переиспользуется коллекция с прошлой отправки
        var count = reader.ReadInt(); 
        
        if (count != 0)
        {
            Cards = new List<RoomPokerCardNetworkModel>();

            for (var i = 0; i < count; i++)
            {
                Cards.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
    }
}