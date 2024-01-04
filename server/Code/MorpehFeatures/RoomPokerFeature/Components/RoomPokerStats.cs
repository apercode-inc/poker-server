using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerStats : IComponent
{
    public byte MaxPlayers;
    public ulong SmallBet;
    public ulong BigBet;
}