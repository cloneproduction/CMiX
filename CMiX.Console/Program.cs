using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
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

            MessageService messageService = serviceProvider.GetRequiredService<MessageService>();
            messageService.StartClient(new Settings("127.0.0.1", 8080));

            CompositionService compositionService = serviceProvider.GetRequiredService<CompositionService>();

            var masterBeat = new MasterBeat();

            var compositionFactory = new CompositionFactory(masterBeat, compositionService);
            var factories = new List<IPrefabFactory>();
            factories.Add(compositionFactory);

            var factory = new PrefabFactory(factories);

            Guid CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var manager = new PrefabManagerBase(CompositionManagerID, compositionService.ProjectRepository, factory);
            Project project = new Project(manager);

            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IService, CompositionService>();
            services.AddSingleton<CompositionService>();
            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MessageService>();
        }
    }
}
