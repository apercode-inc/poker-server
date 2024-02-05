using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.MorpehFeatures.PokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerCards : IComponent
{
    public Queue<CardModel> Cards;
}