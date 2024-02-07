using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.StartTimer;

public struct RoomPokerStartGameSetTimerDataframe : INetworkDataframe
{
    public float WaitTime;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteFloat(WaitTime);
    }

    public void Read(NetFrameReader reader)
    {
        WaitTime = reader.ReadFloat();
    }
}