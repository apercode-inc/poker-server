using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;

namespace server.Code.MorpehFeatures.CleanupDestroyFeature.Systems
{
    public class EntityDestroySystem : SimpleSystem<Destroy>, ICleanupSystem
    {
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                entity.Dispose();
            }
        }
    }
}