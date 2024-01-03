using Scellecs.Morpeh;
using server.Code.GlobalUtils;

namespace server
{
    public class SystemsExecutor
    {
        private World _world;

        public SystemsExecutor(World world)
        {
            _world = world;
        }

        public void Execute()
        {
            _world.FixedUpdate(Time.deltaTime);
            _world.Update(Time.deltaTime);
            _world.LateUpdate(Time.deltaTime);
            _world.CleanupUpdate(Time.deltaTime);
        }
    }
}