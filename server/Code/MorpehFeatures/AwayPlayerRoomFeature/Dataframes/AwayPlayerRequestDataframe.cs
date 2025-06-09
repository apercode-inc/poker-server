using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;

public struct AwayPlayerRequestDataframe : INetworkDataframe
{
    public bool IsAway;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteBool(IsAway);
    }

    public void Read(NetFrameReader reader)
    {
        IsAway = reader.ReadBool();
    }
}