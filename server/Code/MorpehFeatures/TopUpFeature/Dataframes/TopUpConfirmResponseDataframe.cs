using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.TopUpFeature.Dataframes;

public struct TopUpConfirmResponseDataframe : INetworkDataframe
{
    public int PlayerId;
    public long ContributionBalance;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteLong(ContributionBalance);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        ContributionBalance = reader.ReadLong();
    }
}