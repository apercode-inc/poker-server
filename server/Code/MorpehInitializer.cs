using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.ConnectionFeature;

namespace server.Code;

public static class MorpehInitializer
{
    public static void Initialize(World world, SimpleDImple container)
    {
        var groupIndex = 0;
            
        container.AddResolver(type => world.GetReflectionStash(type.GenericTypeArguments[0]), typeof(Stash));
            
        //Storages
        //PlayersFeature.AddStorage(world, ref groupIndex, container);
            
        //Systems
        ConnectionFeature.Add(world, ref groupIndex, container);
        //PlayersFeature.Add(world, ref groupIndex, container);

        //Cleanup
        //CleanupDestroyFeature.Add(world, ref groupIndex, container);
    }
}