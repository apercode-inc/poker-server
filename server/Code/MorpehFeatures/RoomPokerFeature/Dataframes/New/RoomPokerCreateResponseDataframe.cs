using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.New;

public struct RoomPokerCreateResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public CurrencyType CurrencyType; 
    public byte MaxPlayers;
    
    public List<RoomPlayerNetworkModel> PlayerModels;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteInt((int) CurrencyType);
        writer.WriteByte(MaxPlayers);
        
        writer.WriteInt(PlayerModels?.Count ?? 0);

        if (PlayerModels != null)
        {
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
        
        var count = reader.ReadInt();
        
        if (count > 0)
        {
            PlayerModels = new List<RoomPlayerNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                PlayerModels.Add(reader.Read<RoomPlayerNetworkModel>());
            }
        }
    }
}