using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Systems;

namespace server.Code.MorpehFeatures.PlayersFeature;

public static class PlayersFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.NewAndRegister<PlayerStorageSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.New<PlayerNicknameSyncSystem>());
        //systemsGroup.AddSystem(container.New<PlayerNicknameShowTestSystem>());

        world.AddSystemsGroup(index++, systemsGroup);
    }
}