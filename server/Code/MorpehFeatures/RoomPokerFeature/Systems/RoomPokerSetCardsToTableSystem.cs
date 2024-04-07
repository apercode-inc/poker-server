using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerSetCardsToTableSystem : ISystem
{
    private const int CardFlopCount = 3;
    private const int CardTurnCount = 1;
    private const int CardRiverCount = 1;
    
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;
    [Injectable] private Stash<RoomPokerSetCardsToTable> _roomPokerSetCardsToTable;
    [Injectable] private Stash<RoomPokerCardDesk> _roomPokerCardDesk;
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerBank> _roomPokerBank;
    [Injectable] private Stash<RoomPokerSetCardsTickTimer> _roomPokerSetCardsTickTimer;
    [Injectable] private Stash<RoomPokerShowdown> _roomPokerShowdownTimer;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerPlayersGivenBank> _roomPokerPlayersGivenBank;

    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private RoomPokerCardDeskService _cardDeskService;
    [Injectable] private NetFrameServer _server;
    [Injectable] private ConfigsService _configsService;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerId>()
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerCardDesk>()
            .With<RoomPokerSetCardsToTable>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);

            var cards = roomPokerCardsToTable.Cards;
            roomPokerCardsToTable.State++;

            switch (roomPokerCardsToTable.State)
            {
                case CardToTableState.PreFlop:
                    throw new ArgumentOutOfRangeException();
                case CardToTableState.Flop:
                    SetCards(roomEntity, roomPokerCardsToTable.State, cards, CardFlopCount);
                    break;
                case CardToTableState.Turn:
                    SetCards(roomEntity, roomPokerCardsToTable.State, cards, CardTurnCount);
                    break;
                case CardToTableState.River:
                    SetCards(roomEntity, roomPokerCardsToTable.State, cards, CardRiverCount);
                    break;
                case CardToTableState.Showdown:
                    //_roomPokerShowdownTimer.Set(roomEntity);
                    StubToContinueCycleGame(roomEntity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _roomPokerSetCardsToTable.Remove(roomEntity);
        }
    }

    //todo заглушка чтобы просто замкнуть игровой цикл игры в покер (сделать следующую раздачу)
    private void StubToContinueCycleGame(Entity roomEntity)
    {
        ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);

        var playerGivenBank = new FastList<Entity>();
        
        foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
        {
            var player = markedPlayer.Value;

            ref var playerCards = ref _playerCards.Get(player);

            if (playerCards.CardsState == CardsState.Empty)
            {
                continue;
            }

            playerGivenBank.Add(player);
        }
        
        _roomPokerPlayersGivenBank.Set(roomEntity, new RoomPokerPlayersGivenBank
        {
            Players = playerGivenBank,
        });
    }

    private void SetCards(Entity roomEntity, CardToTableState cardToTableState, Queue<CardModel> cards, int cardCount)
    {
        ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);
        ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);

        roomPokerBank.OnTable = roomPokerBank.Total;
        
        var cardsNetworkModels = new List<RoomPokerCardNetworkModel>();
        
        for (var i = 0; i < cardCount; i++)
        {
            if (roomPokerCardDesk.CardDesk.TryRandomRemove(out var cardModel))
            {
                cards.Enqueue(cardModel);
                cardsNetworkModels.Add(new RoomPokerCardNetworkModel
                {
                    Rank = cardModel.Rank,
                    Suit = cardModel.Suit,
                });
            }
            else
            {
                ref var roomPokerId = ref _roomPokerId.Get(roomEntity);
                throw new Exception($"[RoomPokerSetCardsToTableSystem.SetCards] No cards in deck, roomId = {roomPokerId.Value}");
            }
        }
        
        var dataframe = new RoomPokerSetCardsToTableDataframe
        {
            Bank = roomPokerBank.OnTable,
            CardToTableState = cardToTableState,
            Cards = cardsNetworkModels,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
        
        var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
        _roomPokerSetCardsTickTimer.Set(roomEntity, new RoomPokerSetCardsTickTimer
        {
            Value = config.DealingCardTimeToTable,
        });
    }

    public void Dispose()
    {
        _filter = null;
    }
}