using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPayOutPodsSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPaidOutToPlayers> _roomPokerPaidOutToPlayers;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerShowdownChoiceCheck> _roomPokerShowdownChoiceCheck;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;
    
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerShowdownForced> _playerShowdownForced;
    [Injectable] private Stash<PlayerSendWinCombination> _playerSendWinCombination;
    
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private CurrencyPlayerService _currencyPlayerService;
    [Injectable] private PlayerDbService _playerDbService;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPaidOutToPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPaidOutToPlayers = ref _roomPokerPaidOutToPlayers.Get(roomEntity);

            roomPokerPaidOutToPlayers.PaidCooldown += deltaTime;

            if (roomPokerPaidOutToPlayers.PaidOutToPlayers.Count > 0)
            {
                if (roomPokerPaidOutToPlayers.PaidCooldown < roomPokerPaidOutToPlayers.PaidDelay)
                {
                    continue;
                }

                var playerPotModel = roomPokerPaidOutToPlayers.PaidOutToPlayers.First();

                ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
                    
                if (_playerStorage.TryGetPlayerByGuid(playerPotModel.Guid, out var player))
                {
                    ref var playerRoomPoker = ref _playerRoomPoker.Get(player, out var playerRoomExist);

                    if (playerRoomExist && playerRoomPoker.RoomEntity == roomEntity)
                    {
                        _currencyPlayerService.TryGiveBank(roomEntity, player, playerPotModel.ChipsRemaining);

                        if (!_roomPokerOnePlayerRoundGame.Has(roomEntity))
                        {
                            _playerShowdownForced.Set(player);
                        }
                            
                        _playerSendWinCombination.Set(player);
                    }
                    else
                    {
                        SendRefundInfo(playerPotModel, roomEntity);
                        _currencyPlayerService.Give(player, roomPokerStats.CurrencyType,
                            playerPotModel.ChipsRemaining);
                    }
                }
                else
                {
                    switch (roomPokerStats.CurrencyType)
                    {
                        case CurrencyType.Chips:
                            _playerDbService.IncreaseChipsPlayerThreadPool(playerPotModel.Guid, playerPotModel.ChipsRemaining)
                                .Forget();
                            break;
                        case CurrencyType.Gold:
                            break;
                        case CurrencyType.Stars:
                            break;
                    }
                        
                    SendRefundInfo(playerPotModel, roomEntity);
                }
                
                roomPokerPaidOutToPlayers.PaidCooldown = 0;
                roomPokerPaidOutToPlayers.PaidOutToPlayers.Remove(playerPotModel);
            }
            else
            {
                _roomPokerPaidOutToPlayers.Remove(roomEntity);

                ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

                if (roomPokerPlayers.TotalPlayersCount == 1)
                {
                    _roomPokerActive.Remove(roomEntity);
                }

                _roomPokerShowdownChoiceCheck.Set(roomEntity);
            }
        }
    }

    private void SendRefundInfo(PlayerPotModel playerPotModel, Entity roomEntity)
    {
        var dataframe = new RoomPokerCroupierRefundDataframe
        {
            Nickname = playerPotModel.Nickname,
            RefundValue = playerPotModel.ChipsRemaining,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }

    public void Dispose()
    {
        _filter = null;
    }
}