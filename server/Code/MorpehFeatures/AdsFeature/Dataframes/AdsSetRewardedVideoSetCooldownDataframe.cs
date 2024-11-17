using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct AdsSetRewardedVideoSetCooldownDataframe : INetworkDataframe
{
    public string PanelId;
    public bool OnCooldown;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(PanelId);
        writer.WriteBool(OnCooldown);
    }

    public void Read(NetFrameReader reader)
    {
        PanelId = reader.ReadString();
        OnCooldown = reader.ReadBool();
    }
}