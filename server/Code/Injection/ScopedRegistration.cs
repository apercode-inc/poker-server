namespace server.Code.Injection;

public readonly struct ScopedRegistration<T> : IDisposable where T : class
{
    private readonly IInjectionContainer _container;
    private readonly T _instance;

    public ScopedRegistration(IInjectionContainer container, T instance)
    {
        _container = container;
        _instance = instance;
            
        _container.Register(_instance);
    }

    public void Dispose()
    {
        _container.UnRegister(_instance);
    }
}