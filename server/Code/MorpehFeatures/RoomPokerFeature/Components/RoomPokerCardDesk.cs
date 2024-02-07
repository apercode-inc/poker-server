using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerCardDesk : IComponent
{
    public RandomList<CardModel> CardDesk;
}