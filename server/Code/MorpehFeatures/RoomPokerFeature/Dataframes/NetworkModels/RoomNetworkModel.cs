using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomNetworkModel : IWriteable, IReadable
{
    public int Id;
    public byte MaxPlayers;
    public ulong SmallBet;
    public ulong BigBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Id);
        writer.WriteByte(MaxPlayers);
        writer.WriteULong(SmallBet);
        writer.WriteULong(BigBet);
    }

    public void Read(NetFrameReader reader)
    {
        Id = reader.ReadInt();
        MaxPlayers = reader.ReadByte();
        SmallBet = reader.ReadULong();
        BigBet = reader.ReadULong();
    }
}