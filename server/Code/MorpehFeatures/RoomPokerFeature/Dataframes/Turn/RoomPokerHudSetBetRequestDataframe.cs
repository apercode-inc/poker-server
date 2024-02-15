using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.Turn;

public struct RoomPokerHudSetBetRequestDataframe : INetworkDataframe
{
    public long Bet;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(Bet);
    }

    public void Read(NetFrameReader reader)
    {
        Bet = reader.ReadLong();
    }
}