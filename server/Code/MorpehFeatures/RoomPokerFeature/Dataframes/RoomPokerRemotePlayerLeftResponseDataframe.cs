using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerRemotePlayerLeftResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public bool IsAll;
    public int PlayerId;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.WriteBool(IsAll);
        writer.WriteInt(PlayerId);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        IsAll = reader.ReadBool();
        PlayerId = reader.ReadInt();
    }
}