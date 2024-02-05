using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PokerFeature.Dataframes;

public struct PokerSetDealerDataframe : INetworkDataframe
{
    public int PlayerSeat;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(PlayerSeat);
    }

    public void Read(NetFrameReader reader)
    {
        PlayerSeat = reader.ReadInt();
    }
}