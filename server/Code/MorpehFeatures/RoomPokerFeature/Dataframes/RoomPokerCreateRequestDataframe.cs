using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerCreateRequestDataframe : INetworkDataframe
{
    public byte MaxPlayers;
    public CurrencyType CurrencyType;
    public long Contribution;
    public long BigBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteByte(MaxPlayers);
        writer.WriteInt((int) CurrencyType);
        writer.WriteLong(Contribution);
        writer.WriteLong(BigBet);
    }

    public void Read(NetFrameReader reader)
    {
        MaxPlayers = reader.ReadByte();
        CurrencyType = (CurrencyType) reader.ReadInt();
        Contribution = reader.ReadLong();
        BigBet = reader.ReadLong();
    }
}