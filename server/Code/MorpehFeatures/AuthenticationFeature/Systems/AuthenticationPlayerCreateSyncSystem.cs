using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.Components;
using server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.LocalizationFeature;
using server.Code.MorpehFeatures.NotificationFeature.Enums;
using server.Code.MorpehFeatures.NotificationFeature.Systems;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Configs;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Systems;

public class AuthenticationPlayerCreateSyncSystem : IInitializer
{
    [Injectable] private Stash<AuthenticationPlayerCreate> _authenticationPlayerCreate;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;
    [Injectable] private NotificationService _notificationService;
    [Injectable] private PlayerStorage _playerStorage;

    private List<LocalizationParameter> _nicknameLengthParameters;

    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<AuthenticationPlayerCreateResponseDataframe>(Handler);

        _nicknameLengthParameters = new List<LocalizationParameter>
        {
            new LocalizationParameter
            {
                key = "length",
                value = null,
            }
        };
    }

    private void Handler(AuthenticationPlayerCreateResponseDataframe dataframe, int playerId)
    {
        if (!_playerStorage.TryGetPlayerById(playerId, out var playerEntity))
        {
            return;
        }
        
        var config = _configsService.GetConfig<PlayerCreateConfig>(ConfigsPath.PlayerCreate);
        
        if (_playerAuthData.Has(playerEntity))
        {
            _notificationService.Show(playerEntity, AuthenticationLocalizationKeys.AuthPlayerCreateAlreadyExists, NotificationType.Error);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(dataframe.UserId))
        {
            _notificationService.Show(playerEntity, AuthenticationLocalizationKeys.AuthPlayerCreateError, NotificationType.Error);
            return;
        }
        
        if (dataframe.AvatarIndex < 0)
        {
            _notificationService.Show(playerEntity, AuthenticationLocalizationKeys.AuthPlayerCreateIncorrectAvatar, NotificationType.Error);
            return;
        }

        if (dataframe.Nickname.Length < config.NicknameLength)
        {
            _nicknameLengthParameters[0].value = config.NicknameLength.ToString();
            _notificationService.Show(playerEntity,
                AuthenticationLocalizationKeys.AuthPlayerCreateNicknameTooShort,
                NotificationType.Error,
                _nicknameLengthParameters);
            
            return;
        }
        
        _authenticationPlayerCreate.Set(playerEntity, new AuthenticationPlayerCreate
        {
            UserId = dataframe.UserId,
            Nickname = dataframe.Nickname,
            AvatarIndex = dataframe.AvatarIndex,
        });
    }

    public void Dispose()
    {
        _server.Unsubscribe<AuthenticationPlayerCreateResponseDataframe>(Handler);
    }
}