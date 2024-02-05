using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Components;
using server.Code.MorpehFeatures.PokerFeature.Dataframes;
using server.Code.MorpehFeatures.PokerFeature.Enums;
using server.Code.MorpehFeatures.PokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace server.Code.MorpehFeatures.PokerFeature.Systems;

public class PokerInitializeSystem : ISystem
{
    [Injectable] private Stash<PokerInitialize> _pokerInitialize;
    [Injectable] private Stash<PokerActive> _pokerActive;
    [Injectable] private Stash<PokerCardDesk> _pokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<PokerDealingCardsToPlayer> _pokerDealingCardsToPlayer;

    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private PokerCardDeskFactory _cardDeskFactory;

    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayers>()
            .With<PokerInitialize>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _pokerActive.Set(roomEntity);

            if (!_pokerCardDesk.Has(roomEntity))
            {
                _pokerCardDesk.Set(roomEntity, new PokerCardDesk
                {
                    CardDesk = _cardDeskFactory.CreateCardDeskPokerStandard()
                });
            }

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            Entity playerEntity = null;
            if (roomPokerPlayers.MarkedPlayersBySeat.TryGetFirst(out var playerBySeat))
            {
                playerEntity = playerBySeat.Value;
                
                _playerDealer.Set(playerEntity);
                roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.DealerPlayer);

                var dealerSetDataframe = new PokerSetDealerDataframe
                {
                    PlayerSeat = playerBySeat.Key,
                };
                _server.SendInRoom(ref dealerSetDataframe, roomEntity);
            }

            _pokerDealingCardsToPlayer.Set(roomEntity);

            _pokerInitialize.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}