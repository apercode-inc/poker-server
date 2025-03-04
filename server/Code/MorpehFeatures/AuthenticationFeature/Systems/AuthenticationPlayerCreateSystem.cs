using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Components;
using server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;
using server.Code.MorpehFeatures.AuthenticationFeature.DbModels;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Configs;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationPlayerCreateSystem : ISystem
{
    [Injectable] private Stash<AuthenticationPlayerCreate> _authenticationPlayerCreate;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerInitialize> _playerInitialize;
    
    [Injectable] private ThreadSafeFilter<PlayerCreatedSafeContainer> _playerCreatedSafeFilter;
    
    [Injectable] private PlayerDbService _playerDbService;
    [Injectable] private ConfigsService _configsService;
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<AuthenticationPlayerCreate>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var authenticationPlayerCreate = ref _authenticationPlayerCreate.Get(playerEntity);

            var config = _configsService.GetConfig<PlayerCreateConfig>(ConfigsPath.PlayerCreate);

            var newPlayerGuid = Guid.NewGuid().ToString();

            var userModel = new DbUserModel
            {
                unique_id = authenticationPlayerCreate.UserId,
                player_id = newPlayerGuid,
            };
            var playerModel = new DbPlayerModel
            {
                unique_id = newPlayerGuid,
                nickname = authenticationPlayerCreate.Nickname,
                level = 1,
                experience = 0,
                chips = config.StartChips,
                gold = config.StartGold,
                stars = config.StartStars,
                avatar_id = authenticationPlayerCreate.AvatarIndex,
                avart_url = string.Empty,
                registration_date = DateTime.UtcNow,
            };
            
            _authenticationPlayerCreate.Remove(playerEntity);

            if (_playerAuthData.Has(playerEntity))
            {
                continue;
            }
            
            Task.Run(async () =>
            {
                await _playerDbService.InsertPlayerAsync(playerModel);

                _playerCreatedSafeFilter.Add(new PlayerCreatedSafeContainer
                {
                    UserModel = userModel,
                });
            });

            _playerStorage.AddAuth(playerEntity, newPlayerGuid);

            var dataframe = new AuthenticationPlayerCreateCompleteDataframe();
            _server.Send(ref dataframe, playerEntity);
            
            _playerInitialize.Set(playerEntity, new PlayerInitialize
            {
                DbPlayerModel = playerModel,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}