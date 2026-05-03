using System;
using System.Windows;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Animations;
using CMiX.Studio.Services;
using CMiX.Studio.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio
{
    public partial class App : Application
    {
        private MasterBeatAnimationController _animationController;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InjectionBuilder configurationBuilder = new InjectionBuilder();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            configurationBuilder.ConfigureAllServices(serviceCollection);
            serviceCollection.AddSingleton<Studio.Views.MainWindow>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<MainWindowController>();
            serviceCollection.AddSingleton<MainMenu>();
            serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                _ => (collection, onMove) => new ManagerReorderService(collection, onMove));



            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            configurationBuilder.ConfigureWpfTransport(serviceProvider,
                a => Application.Current.Dispatcher.Invoke(a));

            var masterBeat = serviceProvider.GetRequiredService<MasterBeat>();
            _animationController = new MasterBeatAnimationController(masterBeat);

            var mainWindow = serviceProvider.GetRequiredService<Studio.Views.MainWindow>();
            mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            serviceProvider.GetRequiredService<ControlActivationService>().ActivateAll();
            mainWindow.Show();
        }
    }
}
