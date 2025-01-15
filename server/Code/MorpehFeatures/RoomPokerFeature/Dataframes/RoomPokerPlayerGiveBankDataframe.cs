using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerGiveBankDataframe : INetworkDataframe
{
    public int PlayerId;
    public long ContributionBalance;
    public long AllBalance;
    public long Bank;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteLong(ContributionBalance);
        writer.WriteLong(AllBalance);
        writer.WriteLong(Bank);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        ContributionBalance = reader.ReadLong();
        AllBalance = reader.ReadLong();
        Bank = reader.ReadLong();
    }
}