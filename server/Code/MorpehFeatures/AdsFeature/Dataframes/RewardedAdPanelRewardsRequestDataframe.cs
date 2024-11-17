using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct RewardedAdPanelRewardsRequestDataframe : INetworkDataframe
{
    public string PanelId;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(PanelId);
    }

    public void Read(NetFrameReader reader)
    {
        PanelId = reader.ReadString();
    }
}