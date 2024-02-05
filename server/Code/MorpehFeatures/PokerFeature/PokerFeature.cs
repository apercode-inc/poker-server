using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PokerFeature.Factories;
using server.Code.MorpehFeatures.PokerFeature.Systems;

namespace server.Code.MorpehFeatures.PokerFeature;

public static class PokerFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<PokerCardDeskFactory>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.New<PokerStartTimerSetSyncSystem>());

        systemsGroup.AddSystem(container.New<PokerCheckStartSystem>());
        systemsGroup.AddSystem(container.New<PokerStartTimerSystem>());
        systemsGroup.AddSystem(container.New<PokerInitializeSystem>());
        systemsGroup.AddSystem(container.New<PokerDealingCardsToPlayerSystem>());
        systemsGroup.AddSystem(container.New<PokerDealingCardsTimerSystem>());

        systemsGroup.AddSystem(container.New<PokerCheckStopGameSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}