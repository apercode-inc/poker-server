using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;

namespace server.Code.MorpehFeatures.RoomPokerFeature;

public static class RoomPokerFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerStorageSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<RoomPokerCreateRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerJoinRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerLeftRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerListRequestSyncSystem>());

        //systemsGroup.AddSystem(container.New<RoomPokerShowTestSystem>()); //todo test

        systemsGroup.AddSystem(container.New<RoomPokerPlayerJoinSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerLeftSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerDestroySystem>());
        
        systemsGroup.AddSystem(container.New<RoomPokerPlayerCreateSendSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerLocalJoinSendSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerRemoteJoinSendSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}