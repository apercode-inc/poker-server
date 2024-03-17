using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerCardsToTable : IComponent
{
    public CardToTableState State;
    public List<CardModel> Cards;
}