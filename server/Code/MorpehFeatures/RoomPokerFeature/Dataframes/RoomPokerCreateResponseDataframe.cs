using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerCreateResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public CurrencyType CurrencyType; 
    public byte MaxPlayers;
    public long Bank;
    public CardToTableState CardToTableState;
    public List<RoomPokerCardNetworkModel> CardToTableModels;
    public List<RoomPlayerNetworkModel> PlayerModels;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteInt((int) CurrencyType);
        writer.WriteByte(MaxPlayers);
        writer.WriteLong(Bank);
        writer.WriteByte((byte) CardToTableState);

        var hasCards = CardToTableModels != null;
        writer.WriteBool(hasCards);

        if (hasCards)
        {
            writer.WriteInt(CardToTableModels.Count);

            foreach (var card in CardToTableModels)
            {
                writer.Write(card);
            }
        }

        var hasPlayers = PlayerModels != null;
        writer.WriteBool(hasPlayers);

        if (hasPlayers)
        {
            writer.WriteInt(PlayerModels.Count);

            foreach (var player in PlayerModels)
            {
                writer.Write(player);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        CurrencyType = (CurrencyType) reader.ReadInt();
        MaxPlayers = reader.ReadByte();
        Bank = reader.ReadLong();
        CardToTableState = (CardToTableState) reader.ReadByte();

        if (reader.ReadBool())
        {
            var cardsCount = reader.ReadInt();
            CardToTableModels = new List<RoomPokerCardNetworkModel>();

            for (var i = 0; i < cardsCount; i++)
            {
                CardToTableModels.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
        
        if (reader.ReadBool())
        {
            var playerCount = reader.ReadInt();
            PlayerModels = new List<RoomPlayerNetworkModel>();

            for (var i = 0; i < playerCount; i++)
            {
                PlayerModels.Add(reader.Read<RoomPlayerNetworkModel>());
            }
        }
    }
}