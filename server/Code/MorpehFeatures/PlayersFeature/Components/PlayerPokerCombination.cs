using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerPokerCombination : IComponent
{
    public CombinationType CombinationType;
    public int HandStrength;
    public List<CardModel> CombinationCards;
}