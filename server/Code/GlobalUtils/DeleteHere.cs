using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace chess_server.Code.GlobalUtils
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class DeleteHere<T> : ISystem where T : struct, IComponent
    {
        public World World { get; set; }
        private Filter _filter;
        private Stash<T> _stash;

        public void OnAwake()
        {
            _filter = World.Filter.With<T>().Build();
            _stash = World.GetStash<T>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                _stash.Remove(entity);
            }
        }
        
        public void Dispose()
        {
            _stash = null;
            _filter = null;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class DeleteHereFixed<T> : DeleteHere<T>, IFixedSystem where T : struct, IComponent
    {
        
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class DeleteHereLate<T> : DeleteHere<T>, ILateSystem where T : struct, IComponent
    {
        
    }
}