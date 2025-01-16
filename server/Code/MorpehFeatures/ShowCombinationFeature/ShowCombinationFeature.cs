using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ShowCombinationFeature.Systems;

namespace server.Code.MorpehFeatures.ShowCombinationFeature;

public static class ShowCombinationFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddSystem(container.New<ShowCombinationWinPlayerSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}