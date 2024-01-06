using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public Dictionary<Entity, byte> Players; //key - player entity, value - seat
}