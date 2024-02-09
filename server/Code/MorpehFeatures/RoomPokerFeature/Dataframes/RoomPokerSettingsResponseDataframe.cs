using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerSettingsResponseDataframe : INetworkDataframe
{
    public List<int> SeatCounts;
    public List<RoomPokerSettingsBetNetworkModel> Bets;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(SeatCounts?.Count ?? 0);

        if (SeatCounts != null)
        {
            foreach (var seatCount in SeatCounts)
            {
                writer.WriteInt(seatCount);
            }
        }
        
        writer.WriteInt(Bets?.Count ?? 0);

        if (Bets != null)
        {
            foreach (var user in Bets)
            {
                writer.Write(user);
            }
        }
    }


    public void Read(NetFrameReader reader)
    {
        var countForSeats = reader.ReadInt();

        if (countForSeats > 0)
        {
            SeatCounts = new List<int>();
            for (var i = 0; i < countForSeats; i++)
            {
                SeatCounts.Add(reader.ReadInt());
            }
        }
        
        var countForBets = reader.ReadInt();

        if (countForBets > 0)
        {
            Bets = new List<RoomPokerSettingsBetNetworkModel>();
            for (var i = 0; i < countForBets; i++)
            {
                Bets.Add(reader.Read<RoomPokerSettingsBetNetworkModel>());
            }
        }
    }
}