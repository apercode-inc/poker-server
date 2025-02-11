using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.TopUpFeature.Components;
using server.Code.MorpehFeatures.TopUpFeature.Dataframes;

namespace server.Code.MorpehFeatures.TopUpFeature.Systems;

public class TopUpPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerTopUp> _playerTopUp;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;
    [Injectable] private Stash<PlayerAwayRemove> _playerAwayRemove;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerTopUp>()
            .With<PlayerPokerContribution>()
            .With<PlayerRoomPoker>()
            .With<PlayerCurrency>()
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            ref var playerCurrency = ref _playerCurrency.Get(playerEntity);
            ref var playerTopUp = ref _playerTopUp.Get(playerEntity);
            ref var playerPokerContribution = ref _playerPokerContribution.Get(playerEntity);

            var playerTopUpValue = playerTopUp.Value;
            
            _playerTopUp.Remove(playerEntity);

            var roomEntity = playerRoomPoker.RoomEntity;
            
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            
            var topUpValue = roomPokerStats.MinContribution;
            var balance = playerCurrency.CurrencyByType[roomPokerStats.CurrencyType];
            
            var topUpMax = Math.Min(balance, roomPokerStats.Contribution);

            var isFind = false;
            
            while (topUpValue <= topUpMax)
            {
                if (topUpValue == playerTopUpValue)
                {
                    isFind = true;
                    break;
                }
                topUpValue += roomPokerStats.BigBet;
            }
            
            if (balance < roomPokerStats.Contribution && balance == playerTopUpValue)
            {
                isFind = true;
            }

            if (!isFind)
            {
                continue;
            }

            playerPokerContribution.Value = playerTopUpValue;

            ref var playerId = ref _playerId.Get(playerEntity);

            var dataframe = new TopUpConfirmResponseDataframe
            {
                PlayerId = playerId.Id,
                ContributionBalance = playerTopUpValue,
            };
            _server.SendInRoom(ref dataframe, roomEntity);
            
            _playerAwayRemove.Set(playerEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}