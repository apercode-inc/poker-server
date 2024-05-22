using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.PlayersFeature.ThreadSafeContainers;

namespace server.Code.MorpehFeatures.PlayersFeature;

public static class PlayersFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.NewAndRegister<PlayerStorage>());
        systemsGroup.AddInitializer(container.NewAndRegister<PlayerDbService>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        using (container.Scoped(new ThreadSafeFilter<PlayerDbModelThreadSafe>()))
        {
            systemsGroup.AddSystem(container.New<PlayerDbModelRequestSystem>());
            systemsGroup.AddSystem(container.New<PlayerDbModelResponseSystem>());
        }

        systemsGroup.AddSystem(container.New<PlayerInitializeSystem>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
}