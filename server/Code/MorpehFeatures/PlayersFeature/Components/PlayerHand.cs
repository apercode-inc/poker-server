using Scellecs.Morpeh;
using server.Code.MorpehFeatures.PokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerHand : IComponent
{
    public CardModel[] Cards;
}