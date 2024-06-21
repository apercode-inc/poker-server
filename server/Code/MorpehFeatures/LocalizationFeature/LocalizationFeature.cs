using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.LocalizationFeature.Systems;

namespace server.Code.MorpehFeatures.LocalizationFeature;

public static class LocalizationFeature
{
    public static void AddInitializer(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<LocalizationApiKeysRequestsInitializer>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}