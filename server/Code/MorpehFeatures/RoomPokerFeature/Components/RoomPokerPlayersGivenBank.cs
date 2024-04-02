using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayersGivenBank : IComponent
{
    public FastList<Entity> Players;
}