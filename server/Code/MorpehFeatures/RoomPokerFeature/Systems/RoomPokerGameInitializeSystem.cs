using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameInitializeSystem : ISystem
{
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;

    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private RoomPokerCardDeskService _cardDeskService;

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

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            roomPokerPlayers.MarkedPlayersBySeat.ResetMarkers(PokerPlayerMarkerType.ActivePlayer, 
                PokerPlayerMarkerType.NextRoundActivePlayer);

            int dealerPlayerId;

            if (roomPokerPlayers.MarkedPlayersBySeat.TryMoveMarker(PokerPlayerMarkerType.DealerPlayer, 
                    out var markedPlayer))
            {
                var playerEntity = markedPlayer.Value;
                
                ref var playerId = ref _playerId.Get(playerEntity);
                dealerPlayerId = playerId.Id;
            }
            else
            {
                markedPlayer = roomPokerPlayers.MarkedPlayersBySeat.GetFirst();
                    
                var playerEntity = markedPlayer.Value;
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.DealerPlayer);
                    
                ref var playerId = ref _playerId.Get(playerEntity);
                dealerPlayerId = playerId.Id;
            }

            if (roomPokerPlayers.MarkedPlayersBySeat.TryGetNext(PokerPlayerMarkerType.DealerPlayer, out markedPlayer))
            {
                var playerEntity = markedPlayer.Value;
                
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.ActivePlayer);
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.NextRoundActivePlayer);
            }
            
            var dataframe = new RoomPokerSetDealerDataframe
            {
                PlayerId = dealerPlayerId
            };
            _server.SendInRoom(ref dataframe, roomEntity);
            
            _roomPokerDealingCardsToPlayer.Set(roomEntity);
            _roomPokerGameInitialize.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}