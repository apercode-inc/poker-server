using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Services;
using server.Code.MorpehFeatures.RoomPokerFeature.Storages;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;

namespace server.Code.MorpehFeatures.RoomPokerFeature;

public static class RoomPokerFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerSeatsFactory>());
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerCardDeskService>());
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerStorage>());
        systemsGroup.AddInitializer(container.NewAndRegister<RoomPokerService>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<RoomPokerSettingRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerListRequestSyncSystem>());
        
        systemsGroup.AddInitializer(container.New<RoomPokerCreateRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerJoinRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerLeftRequestSyncSystem>());
        systemsGroup.AddInitializer(container.New<RoomPokerStartTimerSetSyncSystem>());

        //systemsGroup.AddSystem(container.New<RoomPokerShowTestSystem>()); //todo test
        
        systemsGroup.AddSystem(container.New<RoomPokerCreateOrJoinSendSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerLeftSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerPlayerDestroySystem>());

        //poker game logic
        systemsGroup.AddSystem(container.New<RoomPokerGameCheckStartSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerGameStartTimerSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerGameInitializeSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerDealingCardsToPlayerSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerDealingCardsTimerSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerSetBlindsSystem>());
        systemsGroup.AddSystem(container.New<RoomPokerTopUpBankSystem>());

        systemsGroup.AddSystem(container.New<RoomPokerSetTurnByPlayerSystem>());
        

        systemsGroup.AddSystem(container.New<RoomPokerCheckStopGameSystem>());
        
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}