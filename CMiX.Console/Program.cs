using CMiX.Core.MessageService;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.Mediator;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;
using MediatR;
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

            //var componentDatabase = serviceProvider.GetService<IComponentDatabase>();

            Settings settings = new Settings("Pouet", "Pouet", "192.168.1.3", 2222);

            //var projectModel = new ProjectModel();
            Project Project = serviceProvider.GetRequiredService<Project>();

            //componentDatabase.AddComponent(Project);

            var communicator = new Communicator(Project);
            Project.SetCommunicator(communicator);


            var messageReceiver = new MessageReceiver(serviceProvider.GetService<IMediator>());
            messageReceiver.Start(settings);
            messageReceiver.RegisterReceiver(communicator);

            System.Console.ReadLine();
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IComponentDatabase, ComponentDatabase>();
            services.AddSingleton<IMediator, Mediator>();
            services.AddMediatR(typeof(AddNewComponentNotification));
            services.AddSingleton<Project>();
        }
    }
}
