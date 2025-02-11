using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.TopUpFeature.Systems;

namespace server.Code.MorpehFeatures.TopUpFeature;

public static class TopUpFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.New<TopUpConfirmRequestSyncSystem>());

        systemsGroup.AddSystem(container.New<TopUpPlayerSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}