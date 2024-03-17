using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerCombination : IComponent
{
    public CombinationType CombinationType;
    public CardModel Card;
}