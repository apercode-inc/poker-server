using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using Server.GlobalUtils;
using server.Code.MorpehFeatures.TestFeature.Systems;
using server.Code.MorpehFeatures.TestFeature.SafeContainers;

namespace server.Code.MorpehFeatures.TestFeature;

public static class TestFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<TestPlayerDbService>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        //systemsGroup.AddSystem(container.New<RoomPokerCheckCardDeskTestSystem>());

        using (container.Scoped(new ThreadSafeFilter<TestSafeContainer>()))
        {
            systemsGroup.AddSystem(container.New<TestConnectionPlayerAndWriteSafeContainerSystem>());
            systemsGroup.AddSystem(container.New<TestCreateRecordInPlayersTable>());
        }
        
        using (container.Scoped(new ThreadSafeFilter<PlayerSafeContainer>()))
        {
            systemsGroup.AddSystem(container.New<PlayerReadDataSystem>());
            systemsGroup.AddSystem(container.New<PlayerWriteDbEntrySystem>());
        }
        
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}