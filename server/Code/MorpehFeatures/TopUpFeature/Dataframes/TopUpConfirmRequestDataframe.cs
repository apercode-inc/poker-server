using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.TopUpFeature.Dataframes;

public struct TopUpConfirmRequestDataframe : INetworkDataframe
{
    public long Value;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(Value);
    }

    public void Read(NetFrameReader reader)
    {
        Value = reader.ReadLong();
    }
}