using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct AdsSetShowRewardedVideoDataframe : INetworkDataframe
{
    public bool CanShow;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteBool(CanShow);
    }

    public void Read(NetFrameReader reader)
    {
        CanShow = reader.ReadBool();
    }
}