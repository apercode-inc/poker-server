using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.DataBaseFeature.Interfaces;
using server.Code.MorpehFeatures.DataBaseFeature.Systems;

namespace server.Code.MorpehFeatures.DataBaseFeature;

public static class DataBaseFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<DataBaseConnector, IDbConnector>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        world.AddSystemsGroup(index++, systemsGroup);
    }
}