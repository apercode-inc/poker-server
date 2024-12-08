using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AdsFeature.Systems;
using server.Code.MorpehFeatures.AdsFeature.ThreadSafeContainers;

namespace server.Code.MorpehFeatures.AdsFeature;

public static class AdsFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<AdsDbService>());
        
        systemsGroup.AddInitializer(container.New<AdsRewardedVideoSyncSystem>());
        systemsGroup.AddInitializer(container.New<AdsRewardedVideoRewardsSyncSystem>());

        systemsGroup.AddSystem(container.New<AdsInitializePlayerSystem>());
        systemsGroup.AddSystem(container.New<AdsRewardedVideoCheckCooldownSystem>());

        using (container.Scoped(new ThreadSafeFilter<PlayerAdsCooldownDbModelThreadSafe>()))
        {
            systemsGroup.AddSystem(container.New<AdsDbCooldownModelRequestSystem>());
            systemsGroup.AddSystem(container.New<AdsDbCooldownModelResponseSystem>());
        }
        
        world.AddSystemsGroup(index++, systemsGroup);
    }

    public static void AddCleanup(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddSystem(container.New<AdsSetPlayerCooldownTimersInDbOnLeaveSystem>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
}