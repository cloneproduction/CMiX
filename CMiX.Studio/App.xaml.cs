using System;
using System.Windows;
using CMiX.Core;
using CMiX.Core.Network;
using CMiX.Core.ViewModels;
using CMiX.Studio.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;

namespace CMiX
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            var serviceCollection = new ServiceCollection();
            configurationBuilder.ConfigureServices(serviceCollection);

            this.ConfigureUIService(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            //serviceProvider.GetRequiredService<ServerManager>().AddNewServer(new ServerSettings("127.0.0.1", 8080));
            var mainWindow = serviceProvider.GetRequiredService<Studio.Views.MainWindow>();

            mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureUIService(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDialogFactory, DialogFactory>();
            services.AddSingleton<Studio.Views.MainWindow>();
        }
    }
}
