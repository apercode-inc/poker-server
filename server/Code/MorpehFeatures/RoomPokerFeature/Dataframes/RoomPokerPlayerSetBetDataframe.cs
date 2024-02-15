using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerSetBetDataframe : INetworkDataframe
{
    public int PlayerId;
    public long Bet;
    public long ContributionBalance;
    public long AllBalance;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteLong(Bet);
        writer.WriteLong(ContributionBalance);
        writer.WriteLong(AllBalance);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        Bet = reader.ReadLong();
        ContributionBalance = reader.ReadLong();
        AllBalance = reader.ReadLong();
    }
}