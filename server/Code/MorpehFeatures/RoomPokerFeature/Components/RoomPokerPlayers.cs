using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public MovingMarkersDictionary<Entity, PokerPlayerMarkerType> Players;
}