using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService<Client>().Start(new Settings("127.0.0.1", 8080));
            var project = serviceProvider.GetRequiredService<Project>();

            System.Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<Project>();
            services.AddSingleton<PrefabRepositories>();
            services.AddSingleton<MasterBeat>();
            services.AddSingleton<CompositionFactory>();
            services.AddSingleton<AssetManager>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();
            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ServerFactory>();

            services.AddSingleton(x => new PrefabFactory(new List<IPrefabFactory> { x.GetRequiredService<CompositionFactory>() }));
            services.AddSingleton(x => new PrefabManagerBase(Guid.Parse("00000000-0000-0000-0000-000000000001"), x.GetRequiredService<PrefabFactory>()));
        }
    }
}
