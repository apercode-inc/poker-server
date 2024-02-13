using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PlayersFeature.Dataframes;

public struct PlayerLocalIdResponseDataframe : INetworkDataframe
{
    public int PlayerId;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
    }
}