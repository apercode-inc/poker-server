using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.AuthenticationFeature.SafeFilters;
using server.Code.MorpehFeatures.AuthenticationFeature.Systems;

namespace server.Code.MorpehFeatures.AuthenticationFeature;

public static class AuthenticationFeature
{
    public static void AddStorage(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        systemsGroup.AddInitializer(container.NewAndRegister<AuthenticationDbService>());
        
        world.AddSystemsGroup(index++, systemsGroup);
    }
    
    public static void Add(World world, ref int index, SimpleDImple container)
    {
        var systemsGroup = world.CreateSystemsGroup();

        using (container.Scoped(new ThreadSafeFilter<UserLoadCompleteSafeContainer>()))
        using (container.Scoped(new ThreadSafeFilter<UserNotFoundSafeContainer>()))
        {
            systemsGroup.AddInitializer(container.New<AuthenticationSyncSystem>());
            
            systemsGroup.AddSystem(container.New<AuthenticationAuthDataSetSystem>());
            systemsGroup.AddSystem(container.New<AuthenticationPlayerCreateSendSystem>());
        }
        
        systemsGroup.AddInitializer(container.New<AuthenticationPlayerCreateSyncSystem>());

        using (container.Scoped(new ThreadSafeFilter<PlayerCreatedSafeContainer>()))
        {
            systemsGroup.AddSystem(container.New<AuthenticationPlayerCreateSystem>());
            systemsGroup.AddSystem(container.New<AuthenticationUserCreateSystem>());
        }

        world.AddSystemsGroup(index++, systemsGroup);
    }
}