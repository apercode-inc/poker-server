using System.Runtime.Serialization;

namespace server.Code.MorpehFeatures.CurrencyFeature.Enums;

public enum CurrencyType
{
    [EnumMember(Value = "chips")] Chips = 0,
    [EnumMember(Value = "gold")] Gold = 1,
    [EnumMember(Value = "stars")] Stars = 2,
}