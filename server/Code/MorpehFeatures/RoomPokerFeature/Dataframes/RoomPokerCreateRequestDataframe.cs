using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerCreateRequestDataframe : INetworkDataframe
{
    public byte MaxPlayers;
    public CurrencyType CurrencyType;
    public long Contribution;
    public long MinContribution;
    public long BigBet;
    public bool IsFastMove;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteByte(MaxPlayers);
        writer.WriteInt((int) CurrencyType);
        writer.WriteLong(Contribution);
        writer.WriteLong(MinContribution);
        writer.WriteLong(BigBet);
        writer.WriteBool(IsFastMove);
    }

    public void Read(NetFrameReader reader)
    {
        MaxPlayers = reader.ReadByte();
        CurrencyType = (CurrencyType) reader.ReadInt();
        Contribution = reader.ReadLong();
        MinContribution = reader.ReadLong();
        BigBet = reader.ReadLong();
        IsFastMove = reader.ReadBool();
    }
}