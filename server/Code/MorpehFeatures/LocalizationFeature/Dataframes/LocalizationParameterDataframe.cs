using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.LocalizationFeature.Dataframes;

public struct LocalizationParameterDataframe : INetworkDataframe
{
    public string key;
    public string value;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(key);
        writer.WriteString(value);
    }

    public void Read(NetFrameReader reader)
    {
        key = reader.ReadString();
        value = reader.ReadString();
    }

    public LocalizationParameter ToParameter()
    {
        return new LocalizationParameter
        {
            key = key,
            value = value
        };
    }

    public static LocalizationParameterDataframe FromParameter(LocalizationParameter parameter)
    {
        return new LocalizationParameterDataframe
        {
            key = parameter.key,
            value = parameter.value
        };
    }
}