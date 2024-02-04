using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PlayersFeature.Dataframes;

public struct CurrencyInitDataframe : INetworkDataframe
{
    public ulong Chips;
    public ulong Gold;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteULong(Chips);
        writer.WriteULong(Gold);
    }

    public void Read(NetFrameReader reader)
    {
        Chips = reader.ReadULong();
        Gold = reader.ReadULong();
    }
}