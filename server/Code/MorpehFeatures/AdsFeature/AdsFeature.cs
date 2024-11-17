using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Systems;

namespace server.Code.MorpehFeatures.AdsFeature;

public static class AdsFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<AdsRewardedVideoSyncSystem>());
        systemsGroup.AddInitializer(container.New<AdsRewardedVideoRewardsSyncSystem>());

        systemsGroup.AddSystem(container.New<AdsSetStartCooldownOnPlayerConnectSystem>());
        systemsGroup.AddSystem(container.New<AdsRewardedVideoCheckCooldownSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}