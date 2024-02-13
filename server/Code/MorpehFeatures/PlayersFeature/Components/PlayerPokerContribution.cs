using Scellecs.Morpeh;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerPokerContribution : IComponent
{
    public CurrencyType CurrencyType;
    public ulong Value;
}