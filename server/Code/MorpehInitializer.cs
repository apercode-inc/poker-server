using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature;
using server.Code.MorpehFeatures.ConfigsFeature;
using server.Code.MorpehFeatures.ConnectionFeature;
using server.Code.MorpehFeatures.PlayersFeature;
using server.Code.MorpehFeatures.RoomPokerFeature;
using server.Code.MorpehFeatures.CurrencyFeature;
using server.Code.MorpehFeatures.DataBaseFeature;
using server.Code.MorpehFeatures.TestFeature;
using server.Code.MorpehFeatures.NotificationFeature;
using server.Code.MorpehFeatures.AuthenticationFeature;

namespace server.Code;

public static class MorpehInitializer
{
    public static void Initialize(World world, SimpleDImple container)
    {
        var groupIndex = 0;
            
        container.AddResolver(type => world.GetReflectionStash(type.GenericTypeArguments[0]), typeof(Stash));
            
        //Storages
        ConfigsFeature.AddStorage(world, ref groupIndex, container);
        DataBaseFeature.AddStorage(world, ref groupIndex, container);
        AuthenticationFeature.AddStorage(world, ref groupIndex, container);
        PlayersFeature.AddStorage(world, ref groupIndex, container);
        RoomPokerFeature.AddStorage(world, ref groupIndex, container);
        CurrencyFeature.AddStorage(world, ref groupIndex, container);
        NotificationFeature.AddStorage(world, ref groupIndex, container);
        TestFeature.AddStorage(world, ref groupIndex, container);

        //Systems
        AuthenticationFeature.Add(world, ref groupIndex, container);
        ConfigsFeature.Add(world, ref groupIndex, container);
        ConnectionFeature.Add(world, ref groupIndex, container);
        PlayersFeature.Add(world, ref groupIndex, container);
        CurrencyFeature.Add(world, ref groupIndex, container);
        RoomPokerFeature.Add(world, ref groupIndex, container);
        TestFeature.Add(world, ref groupIndex, container);

        //Cleanup
        CleanupDestroyFeature.Add(world, ref groupIndex, container);
    }
}