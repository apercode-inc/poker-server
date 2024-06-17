using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.LocalizationFeature.Dataframes;

public struct LocalizationParametersListDataframe : INetworkDataframe
{
    public List<LocalizationParameter> Parameters;
        
    public void Write(NetFrameWriter writer)
    {
        int count = Parameters?.Count ?? 0;
        writer.WriteInt(count);
            
        if (Parameters == null || count == 0)
            return;

        foreach (var parameter in Parameters)
        {
            var dataframe = LocalizationParameterDataframe.FromParameter(parameter);
            dataframe.Write(writer);
        }
    }

    public void Read(NetFrameReader reader)
    {
        Parameters = new List<LocalizationParameter>();
            
        int count = reader.ReadInt();
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            var dataframe = new LocalizationParameterDataframe();
            dataframe.Read(reader);
            Parameters.Add(dataframe.ToParameter());
        }
    }
}