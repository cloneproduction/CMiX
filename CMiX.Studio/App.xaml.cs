using System;
using System.Collections.Generic;
using System.Windows;
using Ceras;
using CMiX.Core.Components;
using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Prefabs;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Studio.Views;
using CMiX.Studio.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;

namespace CMiX
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();


            CompositionService compositionService = ServiceProvider.GetRequiredService<CompositionService>();

            Guid CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var compositionFactory = new CompositionFactory(compositionService.CompositionRepository, compositionService.MasterBeat);

            var factories = new List<IPrefabFactory>();
            factories.Add(compositionFactory);
            var factory = new PrefabFactory(factories);

            var manager = new PrefabManagerBase(CompositionManagerID, compositionService.ProjectRepository, factory);
            Project project = new Project(manager);


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var messageService = new MessageService(new CerasSerializer()); ;
            var mainViewModel = new MainViewModel(project, messageService);// ServiceProvider.GetRequiredService<MainViewModel>();

            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IService, CompositionService>();
            services.AddSingleton<IDialogFactory, DialogFactory>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<IMessageService, MessageService>();
            //services.AddSingleton<IProject, Project>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<CompositionService>();
            services.AddSingleton<Project>();
        }
    }
}



