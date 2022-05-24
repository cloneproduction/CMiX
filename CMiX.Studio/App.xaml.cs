
using Ceras;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Studio.Views;
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
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogFactory, DialogFactory>();
            services.AddSingleton<IDialogTypeLocator, CustomTypeLocator>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IProject, Project>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
        }
    }
}



