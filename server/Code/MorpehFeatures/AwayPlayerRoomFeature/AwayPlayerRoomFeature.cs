using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature;

public static class AwayPlayerRoomFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddSystem(container.New<AwayPlayerMakeSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}