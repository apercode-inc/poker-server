using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.NotificationFeature.Systems;

namespace server.Code.MorpehFeatures.NotificationFeature
{
    public static class NotificationFeature
    {
        public static void AddStorage(World world, ref int index, SimpleDImple container)
        {
            var systemsGroup = world.CreateSystemsGroup();

            systemsGroup.AddInitializer(container.NewAndRegister<NotificationService>());

            world.AddSystemsGroup(index++, systemsGroup);
        }
    }
}