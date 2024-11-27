using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerDealingCardsToPlayer : IComponent
{
    public Queue<Entity> QueuePlayers;
}