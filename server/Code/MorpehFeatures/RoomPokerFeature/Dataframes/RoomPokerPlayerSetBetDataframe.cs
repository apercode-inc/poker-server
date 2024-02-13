using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerSetBetDataframe : INetworkDataframe
{
    public int PlayerId;
    public ulong Bet;
    public ulong ContributionBalance;
    public ulong AllBalance;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteULong(Bet);
        writer.WriteULong(ContributionBalance);
        writer.WriteULong(AllBalance);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        Bet = reader.ReadULong();
        ContributionBalance = reader.ReadULong();
        AllBalance = reader.ReadULong();
    }
}