using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

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
    [Injectable] private Stash<RoomPokerCleanedGame> _roomPokerCleanedGame;
    
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;
    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<PlayerAway> _playerAway;

    [Injectable] private RoomPokerCardDeskService _cardDeskService;
    [Injectable] private RoomPokerService _roomPokerService;
    
    [Injectable] private NetFrameServer _server;

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
            _roomPokerActive.Set(roomEntity);
            _roomPokerBank.Set(roomEntity);

            if (!_roomPokerCardDesk.Has(roomEntity))
            {
                _roomPokerCardDesk.Set(roomEntity, new RoomPokerCardDesk
                {
                    CardDesk = _cardDeskService.CreateCardDeskPokerStandard()
                });
            }
            else
            {
                ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

                _cardDeskService.FillTheDesk(roomPokerCardDesk.CardDesk);
            }

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            Entity dealerPlayer;

            if (roomPokerPlayers.MarkedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.DealerPlayer, 
                    out var markedPlayer))
            {
                dealerPlayer = markedPlayer.Value;
            }
            else
            {
                markedPlayer = roomPokerPlayers.MarkedPlayersBySeat.GetFirst();
                dealerPlayer = markedPlayer.Value;
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(dealerPlayer, PokerPlayerMarkerType.DealerPlayer);
            }

            if (roomPokerPlayers.MarkedPlayersBySeat.TryGetNext(PokerPlayerMarkerType.DealerPlayer, out markedPlayer))
            {
                var playerEntity = markedPlayer.Value;
                
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.ActivePlayer);
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.NextRoundActivePlayer);
            }
            
            _roomPokerService.SetDealerPlayerMarker(roomEntity, dealerPlayer);

            roomPokerPlayers.PlayerPotModels.Clear();
            var playersEntities = new Queue<Entity>();

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntity = playerBySeat.Value;
                
                ref var playerAuthData = ref _playerAuthData.Get(playerEntity);
                ref var playerNickname = ref _playerNickname.Get(playerEntity);

                if (_playerAway.Has(playerEntity))
                {
                    continue;
                }
                
                playersEntities.Enqueue(playerEntity);
                roomPokerPlayers.PlayerPotModels.Add(new PlayerPotModel(playerAuthData.Guid, playerNickname.Value));
            }
            
            _roomPokerSetBlinds.Set(roomEntity, new RoomPokerSetBlinds());
            
            _roomPokerDealingCardsToPlayer.Set(roomEntity, new RoomPokerDealingCardsToPlayer
            {
                QueuePlayers = playersEntities,
            });
            _roomPokerDealingCardsToPlayerSet.Set(roomEntity);
            _roomPokerCleanedGame.Remove(roomEntity);
            _roomPokerGameInitialize.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}