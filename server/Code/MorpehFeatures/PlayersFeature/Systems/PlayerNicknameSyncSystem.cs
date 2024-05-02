using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Dataframes;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerNicknameSyncSystem : IInitializer
{
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerNicknameSetDatabaseTest> _playerNicknameSetDatabaseTest;
    
    [Injectable] private NetFrameServer _server;
    [Injectable] private PlayerStorage _playerStorage;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _server.Subscribe<PlayerSetNicknameDataframe>(Handler);
    }

    private void Handler(PlayerSetNicknameDataframe dataframe, int id)
    {
        if (_playerStorage.TryGetPlayerById(id, out var playerEntity))
        {
            _playerNickname.Set(playerEntity, new PlayerNickname
            {
                Value = dataframe.Nickname,
            });
            _playerNicknameSetDatabaseTest.Set(playerEntity, new PlayerNicknameSetDatabaseTest
            {
                Value = dataframe.Nickname,
            });
        }
    }

    public void Dispose()
    {
        _server.Unsubscribe<PlayerSetNicknameDataframe>(Handler);
    }
}