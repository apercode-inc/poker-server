using Scellecs.Morpeh;
using server.Code.Injection;

namespace server.Code.MorpehFeatures.GameTimeFeature;

public static class GameTimeFeature
{
    public static void AddService(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<GameTimeService>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
}