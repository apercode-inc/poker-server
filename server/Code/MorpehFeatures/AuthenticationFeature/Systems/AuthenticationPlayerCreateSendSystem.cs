using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationPlayerCreateSendSystem : ISystem
{
    [Injectable] private ThreadSafeFilter<UserNotFoundSafeContainer> _safeFilter;

    [Injectable] private NetFrameServer _server;

    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var container in _safeFilter)
        {
            var dataframe = new AuthenticationPlayerCreateRequestDataframe();
            _server.Send(ref dataframe, container.PlayerId);
        }
    }

    public void Dispose()
    {
    }
}