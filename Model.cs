namespace DependencyInjection
{
    public enum LifeTime
    {
        Transient,
        Scoped,
        Singleton,
    }

    public interface IContainerBuilder
    {
        void Register(ServiceDescriptor descriptor);

        IContainer Build();
    }

    public interface IContainer : IDisposable, IAsyncDisposable
    {
        IScope CreateScope();
    }

    public interface IScope : IDisposable, IAsyncDisposable
    {
        object Resolve(Type service);
    }
}
