using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Systems;

namespace server.Code.MorpehFeatures.CleanupDestroyFeature
{
    public static class CleanupDestroyFeature
    {
        public static void Add(World world, ref int index, SimpleDImple container)
        {
            var systemsGroup = world.CreateSystemsGroup();
            
            systemsGroup.AddSystem(container.New<EntityDestroySystem>());

            world.AddSystemsGroup(index++, systemsGroup);
        }
    }
}