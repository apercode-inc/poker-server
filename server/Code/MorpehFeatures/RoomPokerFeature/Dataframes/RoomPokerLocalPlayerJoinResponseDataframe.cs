using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerLocalPlayerJoinResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public byte MaxPlayers;
    public byte Seat;
    
    public List<RoomPlayerNetworkModel> RemotePlayers;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteByte(MaxPlayers);
        writer.WriteByte(Seat);
        
        writer.WriteInt(RemotePlayers?.Count ?? 0);

        if (RemotePlayers != null)
        {
            foreach (var player in RemotePlayers)
            {
                writer.Write(player);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        MaxPlayers = reader.ReadByte();
        Seat = reader.ReadByte();
        
        var count = reader.ReadInt();
        
        if (count > 0)
        {
            RemotePlayers = new List<RoomPlayerNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                RemotePlayers.Add(reader.Read<RoomPlayerNetworkModel>());
            }
        }
    }
}