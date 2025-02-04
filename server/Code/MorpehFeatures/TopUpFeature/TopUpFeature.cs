using Scellecs.Morpeh;
using server.Code.Injection;

namespace server.Code.MorpehFeatures.TopUpFeature;

public static class TopUpFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        world.AddSystemsGroup(index++, systemsGroup);
    }
}