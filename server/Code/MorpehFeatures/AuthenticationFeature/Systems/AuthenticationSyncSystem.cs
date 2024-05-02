using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationSyncSystem : IInitializer
{
    [Injectable] private NetFrameServer _server;

    [Injectable] private ThreadSafeFilter<UserLoadCompleteSafeContainer> _loadCompleteSafeFilter;
    [Injectable] private ThreadSafeFilter<UserNotFoundSafeContainer> _notFoundSafeFilter;

    [Injectable] private AuthenticationDbService _authenticationDbService;

    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<AuthenticationDataframe>(Handler);
    }

    private void Handler(AuthenticationDataframe dataframe, int playerId)
    {
        
        Task.Run(async () =>
        {
            var model = await _authenticationDbService.GetUserAsync(dataframe.UserId);
            
            if (model.Any())
            {
                _loadCompleteSafeFilter.Add(new UserLoadCompleteSafeContainer
                {
                    PlayerId = playerId,
                    PlayerGuid = model.First().player_id,
                });
            }
            else
            {
                _notFoundSafeFilter.Add(new UserNotFoundSafeContainer
                {
                    PlayerId = playerId,
                });
            }
        });
    }

    public void Dispose()
    {
        _server.Unsubscribe<AuthenticationDataframe>(Handler);
    }
}