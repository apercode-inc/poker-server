using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Components;
using server.Code.MorpehFeatures.CurrencyFeature.Dataframe;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerInitializeSystem : ISystem
{
    [Injectable] private Stash<PlayerDbEntry> _playerDbEntry;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerAvatar> _playerAvatar;
    [Injectable] private Stash<PlayerInitialize> _playerInitialize;
    [Injectable] private Stash<PlayerAdsDbCooldownModelRequest> _playerAdsDbCooldownModelRequest;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private Stash<RoomPokerId> _roomPokerId;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .With<PlayerInitialize>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerInitialize = ref _playerInitialize.Get(playerEntity);
            ref var playerAuthData = ref _playerAuthData.Get(playerEntity);

            var model = playerInitialize.DbPlayerModel;
            
            _playerDbEntry.Set(playerEntity, new PlayerDbEntry
            {
                Model = model,
            });

            var currencyByType = new Dictionary<CurrencyType, long>
            {
                [CurrencyType.Chips] = model.chips,
                [CurrencyType.Gold] = model.gold,
                [CurrencyType.Stars] = model.stars,
            };
            
            _playerCurrency.Set(playerEntity, new PlayerCurrency
            {
                CurrencyByType = currencyByType,
            });
            
            var currencyDataframe = new CurrencyInitDataframe
            {
                CurrencyByType = currencyByType,
            };
            _server.Send(ref currencyDataframe, playerEntity);
            
            _playerNickname.Set(playerEntity, new PlayerNickname
            {
                Value = model.nickname,
            });
            _playerAvatar.Set(playerEntity, new PlayerAvatar
            {
                AvatarUrl = model.avart_url,
                AvatarIndex = model.avatar_id,
            });

            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity, out var playerRoomPokerExist);

            var roomPokerIdValue = -1;
            
            if (playerRoomPokerExist)
            {
                ref var roomPokerId = ref _roomPokerId.Get(playerRoomPoker.RoomEntity);
                roomPokerIdValue = roomPokerId.Value;
            }

            var playerInitializeDataframe = new PlayerInitializeDataframe
            {
                Nickname = model.nickname,
                AvatarUrl = model.avart_url,
                AvatarIndex = model.avatar_id,
                Level = model.level,
                Experience = model.experience,
                PlayerGuid = playerAuthData.Guid,
                RoomPokerId = roomPokerIdValue,
            };
            _server.Send(ref playerInitializeDataframe, playerEntity);

            _playerAdsDbCooldownModelRequest.Set(playerEntity);
            _playerInitialize.Remove(playerEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}