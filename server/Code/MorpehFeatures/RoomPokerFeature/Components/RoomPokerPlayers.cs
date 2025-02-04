using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public MovingMarkersDictionary<Entity, PokerPlayerMarkerType> MarkedPlayersBySeat; //Key - seat, Value - player
    public FastList<Entity> AwayPlayers;
    public List<PlayerPotModel> PlayerPotModels;
}