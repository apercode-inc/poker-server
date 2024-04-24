using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.TestFeature.Systems;

namespace server.Code.MorpehFeatures.TestFeature;

public static class TestFeature
{
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        //systemsGroup.AddSystem(container.New<RoomPokerCheckCardDeskTestSystem>());
        systemsGroup.AddSystem(container.New<TestCreateRecordInPlayersTable>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}