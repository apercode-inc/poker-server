using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPokerSettingsBetNetworkModel : IWriteable, IReadable
{
    public ulong BlindBig;
    public ulong Contribution;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteULong(BlindBig);
        writer.WriteULong(Contribution);
    }

    public void Read(NetFrameReader reader)
    {
        BlindBig = reader.ReadULong();
        Contribution = reader.ReadULong();
    }
}