using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct AdsRewardedVideoResultDataframe : INetworkDataframe
{
    public bool IsComplete;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteBool(IsComplete);
    }

    public void Read(NetFrameReader reader)
    {
        IsComplete = reader.ReadBool();
    }
}