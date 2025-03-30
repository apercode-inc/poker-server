using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerDropCardsByPlayerSystem : ISystem
{
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerDropCards> _playerDropCards;
    [Injectable] private Stash<PlayerMoveCompleteFlag> _playerMoveCompleteFlag;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;
    [Injectable] private Stash<RoomPokerPayoutWinnings> _roomPokerPayoutWinnings;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerSetCardsToTable> _roomPokerSetCardsToTable;

    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private RoomPokerService _roomPokerService;
    
    private Filter _filter;
    
    public World World { get; set; }
    
    
    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerRoomPoker>()
            .With<PlayerDropCards>()
            .Build();
    }
    
    public void OnUpdate(float deltaTime)
    {
        foreach (var playerEntity in _filter)
        {
            ref var playerRoomPoker = ref _playerRoomPoker.Get(playerEntity);
            var roomEntity = playerRoomPoker.RoomEntity;
            
            _roomPokerService.DropCards(roomEntity, playerEntity);
            
            _playerMoveCompleteFlag.Set(playerEntity);
            _playerDropCards.Remove(playerEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            var playersWithCardsPlayersCount = 0;
        
            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }
                
                ref var playerCards = ref _playerCards.Get(player);
            
                if (playerCards.CardsState != CardsState.Empty)
                {
                    playersWithCardsPlayersCount++;
                }
            }

            if (playersWithCardsPlayersCount > 1)
            {
                continue;
            }
        
            _roomPokerOnePlayerRoundGame.Set(roomEntity);
            _roomPokerSetCardsToTable.Set(roomEntity);
            
            _roomPokerPayoutWinnings.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}