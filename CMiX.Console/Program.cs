using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            InjectionBuilder configurationBuilder = new InjectionBuilder();

            ServiceCollection serviceCollection = new ServiceCollection();
            configurationBuilder.ConfigureAllServices(serviceCollection);

            var ServiceProvider = serviceCollection.BuildServiceProvider();
            configurationBuilder.ConfigureEngineTransport(ServiceProvider, SyncOptions.Create("127.0.0.1", 6379, 0, "default", "", "cmix:default", "Console"));
            ServiceProvider.GetRequiredService<Project>();
            System.Console.ReadLine();
        }
    }
}
