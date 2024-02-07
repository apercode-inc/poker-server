using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Services;

namespace server.Code.MorpehFeatures.CurrencyFeature;

public static class CurrencyFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();
        
        systemsGroup.AddInitializer(container.NewAndRegister<CurrencyPlayerService>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        //systemsGroup.AddSystem(container.New<CurrencyChangeTESTSystem>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
}