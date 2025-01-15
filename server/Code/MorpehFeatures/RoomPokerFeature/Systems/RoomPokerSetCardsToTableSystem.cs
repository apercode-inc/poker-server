using NetFrame.Server;
using Scellecs.Morpeh;
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
using server.Code.MorpehFeatures.RoomPokerFeature.Services;

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
    [Injectable] private Stash<RoomPokerDetectCombination> _roomPokerDetectCombination;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerOnePlayerRoundGame> _roomPokerOnePlayerRoundGame;

    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private RoomPokerService _roomPokerService;
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
            _roomPokerSetCardsToTable.Remove(roomEntity);
            ref var roomPokerCardsToTable = ref _roomPokerCardsToTable.Get(roomEntity);

            if (_roomPokerOnePlayerRoundGame.Has(roomEntity))
            {
                roomPokerCardsToTable.State = CardToTableState.Showdown;
                SetBankAndSendDataframe(roomEntity, roomPokerCardsToTable.State);
                continue;
            }

            var cards = roomPokerCardsToTable.Cards;
            roomPokerCardsToTable.State++;

            switch (roomPokerCardsToTable.State)
            {
                case CardToTableState.PreFlop:
                    throw new ArgumentOutOfRangeException();
                case CardToTableState.Flop:
                    SetCardsAndBank(roomEntity, roomPokerCardsToTable.State, cards, CardFlopCount);
                    break;
                case CardToTableState.Turn:
                    SetCardsAndBank(roomEntity, roomPokerCardsToTable.State, cards, CardTurnCount);
                    break;
                case CardToTableState.River:
                    SetCardsAndBank(roomEntity, roomPokerCardsToTable.State, cards, CardRiverCount);
                    break;
                case CardToTableState.Showdown:
                    SetBankAndSendDataframe(roomEntity, roomPokerCardsToTable.State);
                    _roomPokerDetectCombination.Set(roomEntity);
                    break;
            }
        }
    }

    private void SetCardsAndBank(Entity roomEntity, CardToTableState cardToTableState, Queue<CardModel> cards, int cardCount)
    {
        ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);

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

        SetBankAndSendDataframe(roomEntity, cardToTableState, cardsNetworkModels);
        
        var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
        _roomPokerSetCardsTickTimer.Set(roomEntity, new RoomPokerSetCardsTickTimer
        {
            Value = config.DealingCardTimeToTable,
        });
    }
    
    private void SetBankAndSendDataframe(Entity roomEntity, CardToTableState cardToTableState, 
        List<RoomPokerCardNetworkModel> cards = null)
    {
        ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);
        roomPokerBank.OnTable = roomPokerBank.Total;
        
        var dataframe = new RoomPokerSetCardsToTableDataframe
        {
            Bank = roomPokerBank.OnTable,
            CardToTableState = cardToTableState,
            Cards = cards,
        };
        _server.SendInRoom(ref dataframe, roomEntity);
    }

    public void Dispose()
    {
        _filter = null;
    }
}