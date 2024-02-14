using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerTurnRequestDataframe : INetworkDataframe
{
    public PokerPlayerTurnType TurnType;
    public float TurnTime;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt((int) TurnType);
        writer.WriteFloat(TurnTime);
    }

    public void Read(NetFrameReader reader)
    {
        TurnType = (PokerPlayerTurnType)reader.ReadInt();
        
    }
}