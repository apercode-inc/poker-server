using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerRemotePlayerJoinResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public RoomPlayerNetworkModel RemotePlayer; 
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.Write(RemotePlayer);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        RemotePlayer = reader.Read<RoomPlayerNetworkModel>();
    }
}