using Scellecs.Morpeh;
using server.Code.MorpehFeatures.AdsFeature.DbModels;

namespace server.Code.MorpehFeatures.AdsFeature.Components;

public struct PlayerAdsDbCooldownModels : IComponent
{
    public List<DbPlayerAdsCooldownModel> Value;
}