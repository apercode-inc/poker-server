using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerLocalPlayerCreateResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public byte MaxPlayers;
    public byte Seat;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteByte(MaxPlayers);
        writer.WriteByte(Seat);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        MaxPlayers = reader.ReadByte();
        Seat = reader.ReadByte();
    }
}