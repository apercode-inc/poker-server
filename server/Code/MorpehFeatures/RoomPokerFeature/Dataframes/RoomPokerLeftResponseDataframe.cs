using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerLeftResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public int PlayerId;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteInt(PlayerId);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        PlayerId = reader.ReadInt();
    }
}