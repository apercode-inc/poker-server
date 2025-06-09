using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.AwayPlayerRoomFeature.Systems;

namespace server.Code.MorpehFeatures.AwayPlayerRoomFeature;

public static class AwayPlayerRoomFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<AwayPlayerRequestSyncSystem>());

        systemsGroup.AddSystem(container.New<AwayPlayerRejoinRoomSystem>());
        systemsGroup.AddSystem(container.New<AwayPlayerAddSystem>());
        systemsGroup.AddSystem(container.New<AwayPlayerTimerTickSystem>());
        systemsGroup.AddSystem(container.New<AwayPlayerRemoveSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}