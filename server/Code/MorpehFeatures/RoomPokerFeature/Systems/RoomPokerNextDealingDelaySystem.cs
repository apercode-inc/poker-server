using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerNextDealingDelaySystem : ISystem
{
    [Injectable] private Stash<RoomPokerNextDealingTimer> _roomPokerNextDealingTimer;
    [Injectable] private Stash<RoomPokerActive> _roomPokerActive;
    [Injectable] private Stash<RoomPokerGameInitialize> _roomPokerGameInitialize;
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;

    [Injectable] private RoomPokerCardDeskService _roomPokerCardDeskService;
    [Injectable] private NetFrameServer _server;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerNextDealingTimer>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            if (!_roomPokerActive.Has(roomEntity))
            {
                _roomPokerNextDealingTimer.Remove(roomEntity);
            }

            ref var roomPokerNextDealingTimer = ref _roomPokerNextDealingTimer.Get(roomEntity);

            roomPokerNextDealingTimer.Value -= deltaTime;

            if (roomPokerNextDealingTimer.Value > 0)
            {
                continue;
            }
            
            _roomPokerCardDeskService.ReturnCardsInDeskToTable(roomEntity);

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;
                
                _roomPokerCardDeskService.ReturnCardsInDeskToPlayer(roomEntity, player);

                ref var playerId = ref _playerId.Get(player);

                var cardsDataframe = new RoomPokerSetCardsByPlayerDataframe
                {
                    CardsState = CardsState.Empty,
                    PlayerId = playerId.Id,
                };
                _server.SendInRoom(ref cardsDataframe, roomEntity);
            }
            
            _roomPokerNextDealingTimer.Remove(roomEntity);
            _roomPokerGameInitialize.Set(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}