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
            ServiceProvider.GetRequiredService<Client>().Start("127.0.0.1", 8080);
            ServiceProvider.GetRequiredService<Project>();
            System.Console.ReadLine();
        }
    }
}
