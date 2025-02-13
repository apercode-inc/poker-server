using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature.Dataframes;

public struct AwayPlayerResetTimerDataframe : INetworkDataframe
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