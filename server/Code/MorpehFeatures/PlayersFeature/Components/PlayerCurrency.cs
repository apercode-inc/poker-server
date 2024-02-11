using Scellecs.Morpeh;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerCurrency : IComponent
{
    public Dictionary<CurrencyType, ulong> CurrencyByType;
}