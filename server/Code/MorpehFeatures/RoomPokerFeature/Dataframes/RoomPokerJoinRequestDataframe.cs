using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerJoinRequestDataframe : INetworkDataframe
{
    public bool IsRejoin;
    public int RoomId;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteBool(IsRejoin);
        writer.WriteInt(RoomId);
    }

    public void Read(NetFrameReader reader)
    {
        IsRejoin = reader.ReadBool();
        RoomId = reader.ReadInt();
    }
}