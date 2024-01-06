using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPlayerNetworkModel : IWriteable, IReadable
{
    public string Nickname;
    public byte Seat;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(Nickname);
        writer.WriteByte(Seat);
    }

    public void Read(NetFrameReader reader)
    {
        Nickname = reader.ReadString();
        Seat = reader.ReadByte();
    }
}