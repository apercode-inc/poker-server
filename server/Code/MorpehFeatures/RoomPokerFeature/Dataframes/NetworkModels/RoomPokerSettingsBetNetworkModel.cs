using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPokerSettingsBetNetworkModel : IWriteable, IReadable
{
    public long BlindBig;
    public long Contribution;
    public long MinContribution;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(BlindBig);
        writer.WriteLong(Contribution);
        writer.WriteLong(MinContribution);
    }

    public void Read(NetFrameReader reader)
    {
        BlindBig = reader.ReadLong();
        Contribution = reader.ReadLong();
        MinContribution = reader.ReadLong();
    }
}