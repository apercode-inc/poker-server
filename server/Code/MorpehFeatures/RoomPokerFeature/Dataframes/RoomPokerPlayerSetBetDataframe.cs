using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerSetBetDataframe : INetworkDataframe
{
    public ulong Bet;
    public int PlayerId;
    public ulong ContributionBalance;
    public ulong AllBalance;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteULong(Bet);
        writer.WriteInt(PlayerId);
        writer.WriteULong(ContributionBalance);
        writer.WriteULong(AllBalance);
    }

    public void Read(NetFrameReader reader)
    {
        Bet = reader.ReadULong();
        PlayerId = reader.ReadInt();
        ContributionBalance = reader.ReadULong();
        AllBalance = reader.ReadULong();
    }
}