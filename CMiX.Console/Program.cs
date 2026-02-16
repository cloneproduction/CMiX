using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
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

            var project = ServiceProvider.GetRequiredService(typeof(MainViewModel));
            //var MainMenu = ServiceProvider.GetService(typeof(MainMenu));

            System.Console.ReadLine();
        }
    }
}
