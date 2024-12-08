using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.AdsFeature.Components;

public struct PlayerAdsRewardedVideoCooldown : IComponent
{
    public List<(string, float)> TimersByPanelId;
}