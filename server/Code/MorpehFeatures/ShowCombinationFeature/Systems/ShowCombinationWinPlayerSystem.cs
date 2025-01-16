using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.ShowCombinationFeature.Dataframes;

namespace server.Code.MorpehFeatures.ShowCombinationFeature.Systems;

public class ShowCombinationWinPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerSendWinCombination> _playerSendWinCombination;
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    private List<RoomPokerCardNetworkModel> _networkCardsModel;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerPokerCombination>()
            .With<PlayerId>()
            .With<PlayerSendWinCombination>()
            .With<PlayerRoomPoker>()
            .Build();
        
        _networkCardsModel = new List<RoomPokerCardNetworkModel>();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            _playerSendWinCombination.Remove(playerEntity);
            _networkCardsModel.Clear();
            
            ref var playerPokerCombination = ref _playerPokerCombination.Get(playerEntity);

            foreach (var card in playerPokerCombination.CombinationCards)
            {
                _networkCardsModel.Add(new RoomPokerCardNetworkModel
                {
                    Rank = card.Rank,
                    Suit = card.Suit,
                });
            }
            
            ref var playerId = ref _playerId.Get(playerEntity);
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);

            var dataframe = new ShowCombinationWinDataframe
            {
                PlayerId = playerId.Id,
                CombinationType = playerPokerCombination.CombinationType,
                Cards = _networkCardsModel,
            };
            _server.SendInRoom(ref dataframe, playerRoomPoker.RoomEntity);
        }
    }
    
    public void Dispose()
    {
        _filter = null;
    }
}