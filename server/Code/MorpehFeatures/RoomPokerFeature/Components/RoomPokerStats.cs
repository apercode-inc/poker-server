using Scellecs.Morpeh;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerStats : IComponent
{
    public byte MaxPlayers;
    public CurrencyType CurrencyType;
    public long Contribution;
    public long MinContribution;
    public long BigBet;
    public float TurnTime;
    public float TurnShowdownTime;
}