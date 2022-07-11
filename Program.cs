using DependencyInjection;

var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
var container = builder.RegisterTransient<IService, Service>()
    .RegisterScoped<Controller, Controller>()
    .Build();

var scope = container.CreateScope();
var controller = scope.Resolve(typeof(Controller));

public interface IAnotherService
{

}

public class AnotherServiceInstance : IAnotherService
{
    private AnotherServiceInstance(){}

    public static AnotherServiceInstance Instance = new();
}

public interface IHelper
{

}

public class Helper : IHelper
{

}

public class Registration
{
    public IContainer ConfigureServices()
    {
        var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
        builder.RegisterTransient<IService, Service>();
        builder.RegisterScoped<Controller, Controller>();
        return builder.Build();
    }
}

public class Controller
{
    private readonly IService _service;

    public Controller(IService service)
    {
        _service = service;
    }

    public void Do()
    {
        Console.WriteLine("Do");
    }
}

public interface IService
{

}

public class Service : IService
{

}