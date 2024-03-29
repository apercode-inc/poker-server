using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;

namespace server.Code.MorpehFeatures.PlayersFeature;

public static class PlayersFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.NewAndRegister<PlayerStorage>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.New<PlayerNicknameSyncSystem>());
        systemsGroup.AddSystem(container.New<PlayerDbModelRequestSystem>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
}