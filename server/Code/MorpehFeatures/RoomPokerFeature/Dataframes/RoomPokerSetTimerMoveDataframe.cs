using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSetTimerMoveDataframe : INetworkDataframe
{
    public int PlayerId;
    public float Time;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerId);
        writer.WriteFloat(Time);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerId = reader.ReadInt();
        Time = reader.ReadFloat();
    }
}