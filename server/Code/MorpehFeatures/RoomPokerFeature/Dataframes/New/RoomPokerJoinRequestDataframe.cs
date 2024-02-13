using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.New;

public struct RoomPokerJoinRequestDataframe : INetworkDataframe
{
    public int RoomId;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
    }
}