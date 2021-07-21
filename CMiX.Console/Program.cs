using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Settings settings = new Settings("192.168.0.192", 2222);
            MessageService messageService = serviceProvider.GetRequiredService<MessageService>();
            messageService.StartClient(settings);

            Project Project = serviceProvider.GetRequiredService<Project>();



            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Project>();
            services.AddSingleton<MessageService>();
        }
    }
}
