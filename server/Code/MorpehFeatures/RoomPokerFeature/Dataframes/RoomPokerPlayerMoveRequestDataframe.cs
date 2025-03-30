using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerMoveRequestDataframe : INetworkDataframe
{
    public PokerPlayerMoveType MoveType;
    public long RequiredBet;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt((int)MoveType);
        writer.WriteLong(RequiredBet);
    }

    public void Read(NetFrameReader reader)
    {
        MoveType = (PokerPlayerMoveType)reader.ReadInt();
        RequiredBet = reader.ReadLong();
    }
}