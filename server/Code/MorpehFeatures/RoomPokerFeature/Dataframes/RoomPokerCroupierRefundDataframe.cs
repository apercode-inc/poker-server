using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerCroupierRefundDataframe : INetworkDataframe
{
    public string Nickname;
    public long RefundValue;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(Nickname);
        writer.WriteLong(RefundValue);
    }

    public void Read(NetFrameReader reader)
    {
        Nickname = reader.ReadString();
        RefundValue = reader.ReadLong();
    }
}