using Scellecs.Morpeh;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerStats : IComponent
{
    public byte MaxPlayers;
    public CurrencyType CurrencyType;
    public ulong Contribution;
    public ulong BigBet;
}