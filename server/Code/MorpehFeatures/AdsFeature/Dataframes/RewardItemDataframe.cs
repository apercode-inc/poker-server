using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct RewardItemDataframe : INetworkDataframe
{
    public long Count;
    public CurrencyType CurrencyType;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteLong(Count);
        writer.WriteByte((byte)CurrencyType);
    }

    public void Read(NetFrameReader reader)
    {
        Count = reader.ReadLong();
        CurrencyType = (CurrencyType)reader.ReadByte();
    }
}