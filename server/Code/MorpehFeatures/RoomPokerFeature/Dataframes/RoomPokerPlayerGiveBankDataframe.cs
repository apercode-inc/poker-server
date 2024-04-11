using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerGiveBankDataframe : INetworkDataframe
{
    public int PlayerId;
    public long ContributionBalance;
    public long AllBalance;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteLong(ContributionBalance);
        writer.WriteLong(AllBalance);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        ContributionBalance = reader.ReadLong();
        AllBalance = reader.ReadLong();
    }
}