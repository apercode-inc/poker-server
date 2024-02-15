using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.CurrencyFeature.Dataframe;

public struct CurrencyInitDataframe : INetworkDataframe
{
    public Dictionary<CurrencyType, long> CurrencyByType;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(CurrencyByType?.Count ?? 0);

        if (CurrencyByType != null)
        {
            foreach (var item in CurrencyByType)
            {
                writer.WriteByte((byte) item.Key);
                writer.WriteLong(item.Value);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        var count = reader.ReadInt();

        if (count > 0)
        {
            CurrencyByType = new Dictionary<CurrencyType, long>();
            for (var i = 0; i < count; i++)
            {
                CurrencyByType.Add((CurrencyType) reader.ReadByte(), reader.ReadLong());
            }
        }
    }
}