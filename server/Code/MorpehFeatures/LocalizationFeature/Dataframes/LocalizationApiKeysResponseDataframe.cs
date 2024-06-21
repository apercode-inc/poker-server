using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.LocalizationFeature.Dataframes;

public struct LocalizationApiKeysResponseDataframe : INetworkDataframe
{
    public string ApiKey;
    public string SpreadsheetId;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(ApiKey);
        writer.WriteString(SpreadsheetId);
    }

    public void Read(NetFrameReader reader)
    {
        ApiKey = reader.ReadString();
        SpreadsheetId = reader.ReadString();
    }
}