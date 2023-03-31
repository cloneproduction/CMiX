using Ceras;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Services;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Scheduling;
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

            CompositionService compositionService = serviceProvider.GetRequiredService<CompositionService>();
            MessageService messageService = serviceProvider.GetRequiredService<MessageService>();
            messageService.StartClient(new Settings("127.0.0.1", 8080));


            Project project = serviceProvider.GetRequiredService<Project>();
            project.CompositionService = compositionService;

            SchedulerManager schedulerManager = new SchedulerManager(project);

            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IPrefabDataBase, PrefabDataBase>();
            services.AddSingleton<IService, CompositionService>();
            services.AddSingleton<CompositionService>();
            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MessageService>();
        }
    }
}
