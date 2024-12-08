using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct AdsRewardedVideoResultDataframe : INetworkDataframe
{
    public string PanelId;
    public bool IsCompleted;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(PanelId);
        writer.WriteBool(IsCompleted);
    }

    public void Read(NetFrameReader reader)
    {
        PanelId = reader.ReadString();
        IsCompleted = reader.ReadBool();
    }
}