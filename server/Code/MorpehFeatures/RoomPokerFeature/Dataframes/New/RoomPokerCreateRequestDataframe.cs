using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.New;

public struct RoomPokerCreateRequestDataframe : INetworkDataframe
{
    public byte MaxPlayers;
    public CurrencyType CurrencyType;
    public ulong Contribution;
    public ulong BigBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteByte(MaxPlayers);
        writer.WriteInt((int) CurrencyType);
        writer.WriteULong(Contribution);
        writer.WriteULong(BigBet);
    }

    public void Read(NetFrameReader reader)
    {
        MaxPlayers = reader.ReadByte();
        CurrencyType = (CurrencyType) reader.ReadInt();
        Contribution = reader.ReadULong();
        BigBet = reader.ReadULong();
    }
}