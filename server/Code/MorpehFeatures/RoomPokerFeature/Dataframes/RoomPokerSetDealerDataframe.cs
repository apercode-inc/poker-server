using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSetDealerDataframe : INetworkDataframe
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