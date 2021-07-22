using Ceras;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;
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
            messageService.StartClient(new Settings("192.168.1.3", 2222));

            Project Project = serviceProvider.GetRequiredService<Project>();

            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Project>();
            services.AddSingleton(new CerasSerializer());
            services.AddSingleton<MessageService>();
        }
    }
}
