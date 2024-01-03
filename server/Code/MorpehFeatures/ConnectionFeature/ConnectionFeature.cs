using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConnectionFeature.Systems;

namespace server.Code.MorpehFeatures.ConnectionFeature;

public static class ConnectionFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddSystem(container.New<StartServerSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}