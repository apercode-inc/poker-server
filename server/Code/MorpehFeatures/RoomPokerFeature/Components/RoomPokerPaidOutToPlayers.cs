using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPaidOutToPlayers : IComponent
{
    public float PaidDelay;
    public float PaidCooldown;
    public List<List<PlayerPotModel>> PaidOutToPlayers;
}