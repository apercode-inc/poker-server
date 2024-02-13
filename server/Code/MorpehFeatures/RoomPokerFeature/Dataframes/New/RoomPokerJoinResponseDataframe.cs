using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.New;

public struct RoomPokerJoinResponseDataframe : INetworkDataframe
{
    public int RoomId;
    public RoomPlayerNetworkModel PlayerModel; 
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(RoomId);
        writer.Write(PlayerModel);
    }

    public void Read(NetFrameReader reader)
    {
        RoomId = reader.ReadInt();
        PlayerModel = reader.Read<RoomPlayerNetworkModel>();
    }
}