using CMiX.Core.DependencyInjection;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Tests
{
    // Mirrors the registrations in App.OnFrameworkInitializationCompleted so tests resolve
    // the same object graph the running app builds, minus the desktop lifetime plumbing.
    public static class TestServiceProviderFactory
    {
        public static IServiceProvider Create()
        {
            var configurationBuilder = new InjectionBuilder();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            configurationBuilder.ConfigureAllServices(serviceCollection);
            serviceCollection.AddSingleton<Views.MainWindow>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<MainWindowController>();
            serviceCollection.AddSingleton<MainMenu>();
            serviceCollection.AddSingleton<IDialogService>(provider => new DialogService(
                new MainWindowDialogManager(new EmptyViewLocator(), new DialogFactory().AddMessageBox()),
                viewModelFactory: type => provider.GetService(type)));
            serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                _ => (collection, onMove) => new ManagerReorderService(collection, onMove));

            return serviceCollection.BuildServiceProvider();
        }

        // Resolves and wires MainWindow the same way App does: DataContext assigned after
        // construction, then every control activated in one pass before anything is shown.
        public static Views.MainWindow CreateMainWindow(IServiceProvider provider)
        {
            var mainWindow = provider.GetRequiredService<Views.MainWindow>();
            mainWindow.DataContext = provider.GetRequiredService<MainViewModel>();
            provider.GetRequiredService<CMiX.Core.ControlActivationService>().ActivateAll();
            return mainWindow;
        }
    }
}
