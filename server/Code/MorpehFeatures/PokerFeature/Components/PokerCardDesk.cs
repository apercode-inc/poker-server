using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.PokerFeature.Models;

namespace server.Code.MorpehFeatures.PokerFeature.Components;

public struct PokerCardDesk : IComponent
{
    public RandomList<CardModel> CardDesk;
}