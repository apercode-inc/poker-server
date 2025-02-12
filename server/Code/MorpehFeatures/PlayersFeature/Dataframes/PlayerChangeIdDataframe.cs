using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PlayersFeature.Dataframes;

public struct PlayerChangeIdDataframe : INetworkDataframe
{
    public int OldId;
    public int NewId;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(OldId);
        writer.WriteInt(NewId);
    }

    public void Read(NetFrameReader reader)
    {
        OldId = reader.ReadInt();
        NewId = reader.ReadInt();
    }
}