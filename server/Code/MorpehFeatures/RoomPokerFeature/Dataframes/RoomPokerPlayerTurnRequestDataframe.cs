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
        writer.WriteInt((int)TurnType);
        writer.WriteLong(RequiredBet);

        var hasBets = RaiseBets != null;
        writer.WriteBool(hasBets);

        if (hasBets)
        {
            writer.WriteInt(RaiseBets.Count);

            foreach (var bet in RaiseBets)
            {
                writer.WriteLong(bet);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        TurnType = (PokerPlayerTurnType)reader.ReadInt();
        RequiredBet = reader.ReadLong();
        RaiseBets = null; //todo временный костыль, нужно исправлять в NetFrame. Переиспользуется коллекция с прошлой отправки

        if (reader.ReadBool())
        {
            var count = reader.ReadInt();
            RaiseBets = new List<long>();

            for (var i = 0; i < count; i++)
            {
                RaiseBets.Add(reader.ReadLong());
            }
        }
    }
}