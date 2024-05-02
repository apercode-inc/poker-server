using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationAuthDataSetSystem : ISystem
{
    [Injectable] private ThreadSafeFilter<UserLoadCompleteSafeContainer> _loadCompleteSafeFilter;

    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var safeContainer in _loadCompleteSafeFilter)
        {
            _playerStorage.AddAuth(safeContainer.PlayerId, safeContainer.PlayerGuid);
        }
    }

    public void Dispose()
    {
    }
}