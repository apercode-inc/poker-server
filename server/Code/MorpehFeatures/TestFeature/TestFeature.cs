using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using Server.GlobalUtils;
using server.Code.MorpehFeatures.TestFeature.Systems;
using server.Code.MorpehFeatures.TestFeature.SafeContainers;

namespace server.Code.MorpehFeatures.TestFeature;

public static class TestFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        world.AddSystemsGroup(index++, systemsGroup);
    }
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        //systemsGroup.AddSystem(container.New<RoomPokerCheckCardDeskTestSystem>());

        using (container.Scoped(new ThreadSafeFilter<PlayerSafeContainer>()))
        {
            systemsGroup.AddSystem(container.New<PlayerReadDataSystem>());
            systemsGroup.AddSystem(container.New<PlayerWriteDbEntrySystem>());
        }
        
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}