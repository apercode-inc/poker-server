using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerDealingCardsByPlayerDataframe : INetworkDataframe
{
    public float DealingCardsTime;
    public List<RoomPokerCardNetworkModel> Cards;
    public List<int> AllPlayersIds;

    public void Write(NetFrameWriter writer)
    { 
        writer.WriteFloat(DealingCardsTime);
        
        var hasCards = Cards != null;
        writer.WriteBool(hasCards);

        if (hasCards)
        {
            writer.WriteInt(Cards.Count);

            foreach (var card in Cards)
            {
                writer.Write(card);
            }
        }

        var hasIds = AllPlayersIds != null;
        writer.WriteBool(hasIds);

        if (hasIds)
        {
            writer.WriteInt(AllPlayersIds.Count);

            foreach (var id in AllPlayersIds)
            {
                writer.WriteInt(id);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        DealingCardsTime = reader.ReadFloat();
            
        Cards = null; //todo временный костыль, нужно исправлять в NetFrame. Переиспользуется коллекция с прошлой отправки
        AllPlayersIds = null; //todo временный костыль, нужно исправлять в NetFrame. Переиспользуется коллекция с прошлой отправки
        
        if (reader.ReadBool())
        {
            var cardsCount = reader.ReadInt();
            Cards = new List<RoomPokerCardNetworkModel>();

            for (var i = 0; i < cardsCount; i++)
            {
                Cards.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }

        if (reader.ReadBool())
        {
            var idsCount = reader.ReadInt();
            AllPlayersIds = new List<int>();

            for (var i = 0; i < idsCount; i++)
            {
                AllPlayersIds.Add(reader.ReadInt());
            }
        }
    }
}