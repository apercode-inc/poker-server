using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerGameInitializeSystem : ISystem
{
    [Injectable] private Stash<RoomPokerGameInitialize> _pokerInitialize;
    [Injectable] private Stash<RoomPokerActive> _pokerActive;
    [Injectable] private Stash<RoomPokerCardDesk> _pokerCardDesk;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerDealingCardsToPlayer> _roomPokerDealingCardsToPlayer;

    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private RoomPokerCardDeskFactory _cardDeskFactory;

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
            _pokerActive.Set(roomEntity);

            if (!_pokerCardDesk.Has(roomEntity))
            {
                _pokerCardDesk.Set(roomEntity, new RoomPokerCardDesk
                {
                    CardDesk = _cardDeskFactory.CreateCardDeskPokerStandard()
                });
            }

            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

            roomPokerPlayers.MarkedPlayersBySeat.ResetAllMarkers();
            
            var count = 0;
            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var playerEntity = playerBySeat.Value;
                var playerSeat = playerBySeat.Key;

                if (count == 0)
                {
                    roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.DealerPlayer);
                    var dataframe = new RoomPokerSetDealerDataframe
                    {
                        PlayerSeat = playerSeat,
                    };
                    _server.SendInRoom(ref dataframe, roomEntity);
                }
                else if (count == 1)
                {
                    roomPokerPlayers.MarkedPlayersBySeat.SetMarker(playerEntity, PokerPlayerMarkerType.ActivePlayer);
                    break;
                }
                
                count++;
            }
            
            _roomPokerDealingCardsToPlayer.Set(roomEntity);

            _pokerInitialize.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}