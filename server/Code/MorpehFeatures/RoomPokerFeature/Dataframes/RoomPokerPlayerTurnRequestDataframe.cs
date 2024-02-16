using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerPlayerTurnRequestDataframe : INetworkDataframe
{
    public PokerPlayerTurnType TurnType;
    public long RequiredBet;
    public List<long> RaiseBets;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt((int) TurnType);
        writer.WriteLong(RequiredBet);
        
        writer.WriteInt(RaiseBets?.Count ?? 0);

        if (RaiseBets != null)
        {
            foreach (var item in RaiseBets)
            {
                writer.WriteLong(item);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        TurnType = (PokerPlayerTurnType)reader.ReadInt();
        RequiredBet = reader.ReadLong();
        
        var count = reader.ReadInt();

        if (count > 0)
        {
            RaiseBets = new List<long>();
            for (var i = 0; i < count; i++)
            {
                RaiseBets.Add(reader.ReadLong());
            }
        }
    }
}