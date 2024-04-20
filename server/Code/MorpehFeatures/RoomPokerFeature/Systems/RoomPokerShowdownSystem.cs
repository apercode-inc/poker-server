using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerShowdownSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerShowdown> _roomPokerShowdown;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerShowOrHideCards> _roomPokerShowOrHideCards;
    [Injectable] private Stash<RoomPokerShowOrHideCardsActivate> _roomPokerShowOrHideCardsActivate;

    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerId> _playerId;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerShowdown>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerBank>()
            .With<RoomPokerPlayersGivenBank>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerPlayersGivenBank = ref _roomPokerPlayersGivenBank.Get(roomEntity);
            ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);

            roomPokerBank.OnTable = roomPokerBank.Total;

            var showdownModels = new List<RoomPokerShowdownNetworkModel>();
            
            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;
                var isWinPlayer = false;
                
                ref var playerCards = ref _playerCards.Get(player);
                
                if (playerCards.CardsState != CardsState.Close)
                {
                    continue;
                }

                foreach (var winPlayer in roomPokerPlayersGivenBank.Players)
                {
                    if (player == winPlayer)
                    {
                        isWinPlayer = true;
                        break;
                    }
                }

                if (isWinPlayer)
                {
                    playerCards.CardsState = CardsState.Open;
                    var cards = playerCards.Cards;

                    var cardsNetworkModels = new List<RoomPokerCardNetworkModel>();

                    foreach (var card in cards)
                    {
                        cardsNetworkModels.Add(new RoomPokerCardNetworkModel
                        {
                            Rank = card.Rank,
                            Suit = card.Suit,
                        });
                    }

                    ref var playerId = ref _playerId.Get(player);
                
                    showdownModels.Add(new RoomPokerShowdownNetworkModel
                    {
                        PlayerId = playerId.Id,
                        Cards = cardsNetworkModels,
                    });
                }
                else
                {
                    ref var roomPokerShowOrHideCards = ref _roomPokerShowOrHideCards.Get(roomEntity, out var exist);

                    if (exist)
                    {
                        roomPokerShowOrHideCards.Players.Enqueue(player);
                    }
                    else
                    {
                        var players = new Queue<Entity>();
                        players.Enqueue(player);
                        
                        _roomPokerShowOrHideCards.Set(roomEntity, new RoomPokerShowOrHideCards
                        {
                            Players = players,
                        });
                    }
                }
            }

            var dataframe = new RoomPokerShowdownDataframe
            {
                Bank = roomPokerBank.OnTable,
                ShowdownModels = showdownModels,
            };
            _server.SendInRoom(ref dataframe, roomEntity);

            _roomPokerShowdown.Remove(roomEntity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}