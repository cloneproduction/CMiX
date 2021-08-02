using Ceras;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Scheduling;
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
            messageService.StartClient(new Settings("192.168.1.4", 8080));

            Project Project = serviceProvider.GetRequiredService<Project>();
            SchedulerManager schedulerManager = new SchedulerManager(Project);
            ComponentManager componentManager = new ComponentManager(messageService, Project);

            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MessageService>();
        }
    }
}
