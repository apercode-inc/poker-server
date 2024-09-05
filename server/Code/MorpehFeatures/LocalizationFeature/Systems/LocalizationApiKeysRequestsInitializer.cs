using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.LocalizationFeature.Dataframes;

namespace server.Code.MorpehFeatures.LocalizationFeature.Systems;

public class LocalizationApiKeysRequestsInitializer : IInitializer
{
    private const string ApiKey = "AIzaSyACuMocnZAG_mqlzss0KyVSkUBN7Nk4Xf4";
    private const string SpreadSheetId = "1YWco69Y6i192jLeZ38WTR5uMOhS5-ZpGnMbrJ8FmTu4";
    
    [Injectable] private NetFrameServer _server;

    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<LocalizationApiKeysRequestDataframe>(ProcessApiKeysRequest);
    }

    private void ProcessApiKeysRequest(LocalizationApiKeysRequestDataframe request, int clientId)
    {
        var response = new LocalizationApiKeysResponseDataframe
        {
            ApiKey = ApiKey,
            SpreadsheetId = SpreadSheetId
        };
        _server.Send(ref response, clientId);
    }

    public void Dispose()
    {
        
    }
}