using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;

namespace server.Code.MorpehFeatures.RoomPokerFeature;

public static class RoomPokerFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.New<RoomPokerCreateRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerJoinRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerLeftRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerListRequestSyncSystem>());
        
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerStorageSystem>());

        systemsGroup.AddSystem(container.New<RoomPokerPlayerJoinSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerLeftSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}