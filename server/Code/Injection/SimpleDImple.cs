using System.Reflection;

namespace server.Code.Injection;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property)]
public class InjectableAttribute : Attribute
{
    internal static readonly Type AttributeType = typeof(InjectableAttribute);
}

public class SimpleDImple : IInjectionContainer
{
    private readonly Dictionary<Type, object> _registrations = new();
    private readonly Dictionary<Type, Func<Type, object>> _resolvers = new();
    

    public SimpleDImple(bool injectSelf = true)
    {
        if (injectSelf)
            Register(this); // Allow injecting itself
    }

    private object Get(Type type) 
    {
        object instance;
        if (_registrations.TryGetValue(type, out instance)) 
        {
            return instance;
        }

        var baseType = type.BaseType;
        if (baseType != null) 
        {
            if (_resolvers.TryGetValue(baseType, out var resolver)) 
            {
                instance = resolver?.Invoke(type);
                if (instance != null) 
                {
                    return instance;
                }
            }
        }
        throw new InvalidOperationException($"SimpleDi: No known dependency of type = {type}");
    }
    
    public void UnRegister<T>(T instance) where T : class
    {
        var type = typeof(T);
        _registrations.Remove(type);
    }


    public void Register<T>(T instance) where T : class
    {
        var type = typeof(T);
        if (_registrations.ContainsKey(type))
            throw new InvalidOperationException($"SimpleDi: Dependency of type = {type} already exists");
        
        _registrations.Add(type, instance);
    }
    
    public void UnRegister<T>(T instance, Type type) where T : class
    {
        _registrations.Remove(type);
    }
    
    public void Register<T>(T instance, Type type) where T : class
    {
        if (_registrations.ContainsKey(type))
            throw new InvalidOperationException($"SimpleDi: Dependency of type = {type} already exists");
        
        _registrations.Add(type, instance);
    }
    
    public ScopedRegistration<T> Scoped<T>(T instance) where T : class
    {
        return new ScopedRegistration<T>(this, instance);
    }
    
    public void AddResolver(Func<Type, object> resolver, Type baseType)
    {
        if (_resolvers.ContainsKey(baseType))
            throw new InvalidOperationException($"SimpleDi: Resolver of type = {baseType} already exists");
        
        _resolvers.Add(baseType, resolver);
    }

    public T Get<T>() where T : class
    {
        return (T) Get(typeof(T));
    }

    private void InjectFields<T>(T instance, Type type)
    {
        var fields = type.GetRuntimeFields().Where(x => x.IsDefined(InjectableAttribute.AttributeType));
        foreach (var item in fields)
        {
            item.SetValue(instance, Get(item.FieldType));
        }
    }

    private void InjectProperties<T>(T instance, Type type)
    {
        var properties = type.GetRuntimeProperties().Where(x => x.CanWrite && x.IsDefined(InjectableAttribute.AttributeType));
        foreach (var item in properties)
        {
            item.SetValue(instance, Get(item.PropertyType));
        }
    }

    public T New<T>() where T : class, new()
    {
        var instance = new T();
        var type = typeof(T);
        InjectFields(instance, type);
        InjectProperties(instance, type);
        return instance;
    }
    
    public T NewAndRegister<T>() where T : class, new()
    {
        var instance = New<T>();
        Register(instance);
        return instance;
    }
    
    public T1 NewAndRegister<T1, T2>() where T1 : class, T2, new() where T2 : class
    {
        var obj = New<T1>();
        Register<T2>(obj);
        return obj;
    }

    [Obsolete("Use New<T> instead. New<T> does NOT support constructor injection")]
    public T Create<T>(Type type) where T : class
    {
        var constructors = type.GetConstructors();
        var constructorInfo = constructors.FirstOrDefault(constructor => constructor.IsDefined(InjectableAttribute.AttributeType));

        T instance;

        if (constructorInfo != null)
        {
            var argsTypes = constructorInfo.GetParameters().Select(p => p.ParameterType);
            var dependencies = argsTypes.Select(Get).ToArray();
            instance = (T) Activator.CreateInstance(type, dependencies);
        }
        else
        {
            try
            {
                instance = (T) Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                constructorInfo = constructors.FirstOrDefault();
                var argsTypes = constructorInfo.GetParameters().Select(p => p.ParameterType);
                var dependencies = argsTypes.Select(Get).ToArray();
                instance = (T) Activator.CreateInstance(type, dependencies);
            }
        }

        InjectFields(instance, type);
        InjectProperties(instance, type);

        return instance;
    }

    [Obsolete("Use New<T> instead. New<T> does NOT support constructor injection")]
    public T Create<T>() where T : class
    {
        var type = typeof(T);
        return Create<T>(type);
    }
    
    [Obsolete("Use NewAndRegister<T> instead. NewAndRegister<T> does NOT support constructor injection")]
    public T CreateAndRegister<T>() where T : class
    {
        var type = typeof(T);
        var obj = Create<T>(type);
        Register(obj);
        return obj;
    }
    
    [Obsolete("Use NewAndRegister<T1, T2> instead. NewAndRegister<T1, T2> does NOT support constructor injection")]
    public T1 CreateAndRegister<T1, T2>() where T1 : class, T2 where T2 : class
    {
        var obj = Create<T1>();
        Register<T2>(obj);
        return obj;
    }
    
    public Dictionary<Type, object> GetRegistrations()
    {
        return _registrations;
    }

    public void CopyRegistrationsFrom(IInjectionContainer otherContainer)
    {
        foreach (var pair in otherContainer.GetRegistrations())
        {
            _registrations.Add(pair.Key, pair.Value);
        }
    }
}