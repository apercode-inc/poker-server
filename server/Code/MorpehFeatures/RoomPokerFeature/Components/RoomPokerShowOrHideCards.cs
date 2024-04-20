using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerShowOrHideCards : IComponent
{
    public Queue<Entity> Players;
}