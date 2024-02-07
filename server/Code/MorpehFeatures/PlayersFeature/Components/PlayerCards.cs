using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerCards : IComponent
{
    public Queue<CardModel> Cards;
}