using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPokerSettingsBetNetworkModel : IWriteable, IReadable
{
    public long BlindBig;
    public long Contribution;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(BlindBig);
        writer.WriteLong(Contribution);
    }

    public void Read(NetFrameReader reader)
    {
        BlindBig = reader.ReadLong();
        Contribution = reader.ReadLong();
    }
}