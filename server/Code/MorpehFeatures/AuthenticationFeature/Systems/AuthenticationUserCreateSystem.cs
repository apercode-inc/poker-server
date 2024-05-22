using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationUserCreateSystem : ISystem
{
    [Injectable] private AuthenticationDbService _authenticationDbService;
    
    [Injectable] private ThreadSafeFilter<PlayerCreatedSafeContainer> _playerCreatedSafeFilter;

    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var container in _playerCreatedSafeFilter)
        {
            _authenticationDbService.InsertUserThreadPool(container.UserModel).Forget();
        }
    }

    public void Dispose()
    {
    }
}