using NetFrame.Server;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
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

    [Injectable] private RoomPokerCardDeskService _cardDeskService;
    [Injectable] private NetFrameServer _server;

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
                    SetCards(roomEntity, cards, CardFlopCount);
                    break;
                case CardToTableState.Turn:
                    SetCards(roomEntity, cards, CardTurnCount);
                    break;
                case CardToTableState.River:
                    SetCards(roomEntity, cards, CardRiverCount);
                    break;
                case CardToTableState.Showdown:
                    //TODO передача управления системе (или системам) которые сравнивают комбинации
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _roomPokerSetCardsToTable.Remove(roomEntity);
        }
    }

    private void SetCards(Entity roomEntity, FastList<CardModel> cards, int cardCount)
    {
        ref var roomPokerCardDesk = ref _roomPokerCardDesk.Get(roomEntity);
        ref var roomPokerBank = ref _roomPokerBank.Get(roomEntity);
        
        var cardsNetworkModels = new List<RoomPokerCardNetworkModel>();
        
        for (var i = 0; i < cardCount; i++)
        {
            if (roomPokerCardDesk.CardDesk.TryRandomRemove(out var cardModel))
            {
                cards.Add(cardModel);
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
            Bank = roomPokerBank.Value,
            Cards = cardsNetworkModels,
        };
        _server.SendInRoom(ref dataframe, roomEntity);

        //todo надо сделать какой то таймер чтобы показать раскладку и дальше передавать ходы игрокам
    }

    public void Dispose()
    {
        _filter = null;
    }
}