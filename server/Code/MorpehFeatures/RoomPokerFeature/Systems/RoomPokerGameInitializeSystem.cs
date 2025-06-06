using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameInitializeSystem : ISystem
{
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayerSet> _roomPokerDealingCardsToPlayerSet;
    [Injectable] private Stash<RoomPokerSetBlinds> _roomPokerSetBlinds;
    [Injectable] private Stash<RoomPokerTransferDealer> _roomPokerTransferDealer;
    [Injectable] private Stash<RoomPokerCleanedGame> _roomPokerCleanedGame;
    
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private RoomPokerCardDeskService _cardDeskService;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<RoomPokerGameInitialize>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerGameInitialize.Remove(roomEntity);
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            if (roomPokerPlayers.TotalPlayersCount <= 1)
            {
                continue;
            }
            
            _roomPokerCardDesk.Set(roomEntity, new RoomPokerCardDesk
            {
                CardDesk = _cardDeskService.CreateCardDeskPokerStandard()
            });

            roomPokerPlayers.PlayerPotModels.Clear();
            var playersEntities = new Queue<Entity>();

            foreach (var player in roomPokerPlayers.PlayersBySeat)
            {
                if (player.IsNullOrDisposed())
                {
                    continue;
                }
                
                ref var playerAuthData = ref _playerAuthData.Get(player);
                ref var playerNickname = ref _playerNickname.Get(player);
                
                roomPokerPlayers.PlayerPotModels.Add(new PlayerPotModel(playerAuthData.Guid, playerNickname.Value));

                if (_playerAway.Has(player))
                {
                    continue;
                }
                
                playersEntities.Enqueue(player);
            }

            _roomPokerDealingCardsToPlayer.Set(roomEntity, new RoomPokerDealingCardsToPlayer
            {
                QueuePlayers = playersEntities,
            });
            _roomPokerDealingCardsToPlayerSet.Set(roomEntity);
            _roomPokerCleanedGame.Remove(roomEntity);
            
            _roomPokerSetBlinds.Set(roomEntity);
            _roomPokerActive.Set(roomEntity);
            _roomPokerBank.Set(roomEntity);
            _roomPokerTransferDealer.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}