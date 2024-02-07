using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerSetBetDataframe : INetworkDataframe
{
    public ulong Bet;
    public int PlayerId;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteULong(Bet);
        writer.WriteInt(PlayerId);
    }

    public void Read(NetFrameReader reader)
    {
        Bet = reader.ReadULong();
        PlayerId = reader.ReadInt();
    }
}