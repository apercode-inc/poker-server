using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PokerFeature.Dataframes.StartTimer;

public struct PokerStartGameSetTimerDataframe : INetworkDataframe
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