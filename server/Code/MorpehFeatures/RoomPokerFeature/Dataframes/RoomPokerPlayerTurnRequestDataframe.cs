using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerTurnRequestDataframe : INetworkDataframe
{
    public PokerPlayerTurnType TurnType;
    public long RequiredBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt((int)TurnType);
        writer.WriteLong(RequiredBet);
    }

    public void Read(NetFrameReader reader)
    {
        TurnType = (PokerPlayerTurnType)reader.ReadInt();
        RequiredBet = reader.ReadLong();
    }
}