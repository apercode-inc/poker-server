using System.Runtime.CompilerServices;
using Scellecs.Morpeh;

namespace server.Code.GlobalUtils
{
    public static class MorpehSystemsGroupExtensions
    {
        private static readonly Type FixedType = typeof(IFixedSystem);
        private static readonly Type LateType = typeof(ILateSystem);
        
        /// <summary>Deletes T component from all entities ("Update" by default, use T1, T2 overload for ILateSystem/IFixedSystem)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeleteHere<T>(this SystemsGroup systemsGroup) where T : struct, IComponent
        {
            systemsGroup.AddSystem(new DeleteHere<T>());
        }

        /// <summary>Deletes T component from all entities</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeleteHere<T1, T2>(this SystemsGroup systemsGroup) where T1 : ISystem where T2 : struct, IComponent
        {
            var type = typeof(T1);
            if (type == FixedType)
            {
                systemsGroup.AddSystem(new DeleteHereFixed<T2>());
                return;
            }

            if (type == LateType)
            {
                systemsGroup.AddSystem(new DeleteHereLate<T2>());
                return;
            }
            
            systemsGroup.AddSystem(new DeleteHere<T2>());
        }
    }
}