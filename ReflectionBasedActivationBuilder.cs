using System.Linq.Expressions;
using System.Reflection;

namespace DependencyInjection;

public interface IActivationBuilder
{
    public Func<IScope, object> BuildActivation(ServiceDescriptor descriptor);
}

public abstract class BaseActivationBuilder : IActivationBuilder
{
    public Func<IScope, object> BuildActivation(ServiceDescriptor descriptor)
    {
        var tb = (TypeBasedServiceDescriptor)descriptor;
        var ctor = tb.ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
        var args = ctor.GetParameters();

        return BuildActivationInternal(tb, ctor, args, descriptor);
    }

    protected abstract Func<IScope, object> BuildActivationInternal(TypeBasedServiceDescriptor tb, ConstructorInfo ctor, ParameterInfo[] args, ServiceDescriptor descriptor);
}

public class ReflectionBasedActivationBuilder : BaseActivationBuilder
{
    protected override Func<IScope, object> BuildActivationInternal(TypeBasedServiceDescriptor tb, ConstructorInfo ctor, ParameterInfo[] args,
        ServiceDescriptor descriptor)
    {
        return s =>
        {
            var argsForCtor = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argsForCtor[i] = s.Resolve(args[i].ParameterType);
            }

            return ctor.Invoke(argsForCtor);
        };
    }
}

public class LambdaBasedActivationBuilder : BaseActivationBuilder
{
    private static readonly MethodInfo ResolveMethod = typeof(IScope).GetMethod("Resolve");
    protected override Func<IScope, object> BuildActivationInternal(TypeBasedServiceDescriptor tb, ConstructorInfo ctor, ParameterInfo[] args,
        ServiceDescriptor descriptor)
    {
        var scopeParameter = Expression.Parameter(typeof(IScope), "scope");

        var ctorArgs= args.Select(x =>
            Expression.Convert(Expression.Call(scopeParameter, ResolveMethod,
             Expression.Constant(x.ParameterType)), x.ParameterType));
        var @new = Expression.New(ctor, ctorArgs);

        var lambda = Expression.Lambda<Func<IScope, object>>(@new, scopeParameter);
        return lambda.Compile();
    }
}