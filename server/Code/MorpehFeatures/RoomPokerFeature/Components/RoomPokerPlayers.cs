using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.PokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public MovingMarkersDictionary<Entity, PokerPlayerMarkerType> MarkedPlayersBySeat; //Key - seat, Value - player
}