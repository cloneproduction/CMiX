using AutoMapper;
using Ceras;
using CMiX.Core.Mapper;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.ViewModels.Services;
using CMiX.Studio.Views;
using CMiX.Studio.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using MvvmDialogs.DialogTypeLocators;
using System;
using System.Windows;

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
            Project project = new Project(compositionService);


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.Project = project;
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });
            config.AssertConfigurationIsValid();

            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IService, CompositionService>();
            services.AddSingleton<IPrefabDataBase, PrefabDataBase>();
            services.AddSingleton<IDialogFactory, DialogFactory>();
            //services.AddSingleton<IDialogTypeLocator, CustomTypeLocator>();

            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IProject, Project>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<CompositionService>();
            services.AddSingleton<Project>();
        }
    }
}



