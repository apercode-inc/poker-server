using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerCreateRequestDataframe : INetworkDataframe
{
    public byte MaxPlayers;
    public ulong SmallBet;
    public ulong BigBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteByte(MaxPlayers);
        writer.WriteULong(SmallBet);
        writer.WriteULong(BigBet);
    }

    public void Read(NetFrameReader reader)
    {
        MaxPlayers = reader.ReadByte();
        SmallBet = reader.ReadULong();
        BigBet = reader.ReadULong();
    }
}