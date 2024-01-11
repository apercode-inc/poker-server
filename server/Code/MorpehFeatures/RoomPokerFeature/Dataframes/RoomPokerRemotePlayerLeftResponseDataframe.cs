using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerRemotePlayerLeftResponseDataframe : INetworkDataframe
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