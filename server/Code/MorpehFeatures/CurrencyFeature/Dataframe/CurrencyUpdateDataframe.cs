using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.CurrencyFeature.Dataframe;

public struct CurrencyUpdateDataframe : INetworkDataframe
{
    public CurrencyType Type;
    public long Value;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteByte((byte)Type);
        writer.WriteLong(Value);
    }

    public void Read(NetFrameReader reader)
    {
        Type = (CurrencyType) reader.ReadByte();
        Value = reader.ReadLong();
    }
}