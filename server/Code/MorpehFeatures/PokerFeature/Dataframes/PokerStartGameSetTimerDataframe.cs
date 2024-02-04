using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PokerFeature.Dataframes;

public struct PokerStartGameSetTimerDataframe : INetworkDataframe
{
    public int WaitTime;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(WaitTime);
    }

    public void Read(NetFrameReader reader)
    {
        WaitTime = reader.ReadInt();
    }
}