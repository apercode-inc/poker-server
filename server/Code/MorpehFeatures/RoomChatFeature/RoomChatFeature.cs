using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomChatFeature.Systems;

namespace server.Code.MorpehFeatures.RoomChatFeature;

public static class RoomChatFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.NewAndRegister<RoomChatMessagesSyncInitializer>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}