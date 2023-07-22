using Ceras;
using CMiX.Core.Components;
using CMiX.Core.Prefab;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Scheduling;
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
            services.AddSingleton<IService, CompositionService>();
            services.AddSingleton<CompositionService>();
            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MessageService>();
        }
    }
}
