using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace server.Code.GlobalUtils
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class SimpleSystem<T> where T : struct, IComponent
    {
        public World World { get; set; }
        protected Filter _filter;

        public virtual void OnAwake()
        {
            _filter = World.Filter.With<T>().Build();
        }

        public virtual void Dispose()
        {
            _filter = null;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class SimpleSystem<T1, T2> where T1 : struct, IComponent where T2 : struct, IComponent
    {
        public World World { get; set; }
        protected Filter _filter;

        public virtual void OnAwake()
        {
            _filter = World.Filter.With<T1>().With<T2>().Build();
        }

        public virtual void Dispose()
        {
            _filter = null;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class SimpleSystem<T1, T2, T3> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent
    {
        public World World { get; set; }
        protected Filter _filter;

        public virtual void OnAwake()
        {
            _filter = World.Filter.With<T1>().With<T2>().With<T3>().Build();
        }

        public virtual void Dispose()
        {
            _filter = null;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class SimpleSystem<T1, T2, T3, T4> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent
    {
        public World World { get; set; }
        protected Filter _filter;

        public virtual void OnAwake()
        {
            _filter = World.Filter.With<T1>().With<T2>().With<T3>().With<T4>().Build();
        }

        public virtual void Dispose()
        {
            _filter = null;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class SimpleSystem<T1, T2, T3, T4, T5> where T1 : struct, IComponent where T2 : struct, IComponent where T3 : struct, IComponent where T4 : struct, IComponent where T5 : struct, IComponent
    {
        public World World { get; set; }
        protected Filter _filter;

        public virtual void OnAwake()
        {
            _filter = World.Filter.With<T1>().With<T2>().With<T3>().With<T4>().With<T5>().Build();
        }

        public virtual void Dispose()
        {
            _filter = null;
        }
    }
}