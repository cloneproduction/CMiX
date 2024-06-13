using CMiX.Core;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            ServiceCollection serviceCollection = new ServiceCollection();
            configurationBuilder.ConfigureServices(serviceCollection);

            var ServiceProvider = serviceCollection.BuildServiceProvider();
            //ServiceProvider.GetRequiredService<Client>().Start(new ServerSettings("127.0.0.1", 8080));

            var project = ServiceProvider.GetService(typeof(MainViewModel));
            //var MainMenu = ServiceProvider.GetService(typeof(MainMenu));

            System.Console.ReadLine();
        }
    }
}
