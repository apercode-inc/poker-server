using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct AdsSetRewardedVideoSetCooldownDataframe : INetworkDataframe
{
    public string PanelId;
    public float RemainingSeconds;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(PanelId);
        writer.WriteFloat(RemainingSeconds);
    }

    public void Read(NetFrameReader reader)
    {
        PanelId = reader.ReadString();
        RemainingSeconds = reader.ReadFloat();
    }
}