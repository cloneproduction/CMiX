using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

        // The common starting point for a test that exercises the app through its real window:
        // build a provider, resolve and show MainWindow, and pump once so the initial layout and
        // binding pass settles before the test starts driving it.
        public static (IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel) ShowMainWindow()
        {
            var provider = Create();
            var window = CreateMainWindow(provider);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (provider, window, provider.GetRequiredService<MainViewModel>());
        }

        // Two dispatcher passes, the idiom this suite uses wherever a UI visible change needs its
        // deferred bindings or layout to settle before assertions run.
        public static void Pump()
        {
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();
        }

        // The app has exactly one top level TabControl, the same one every test that drives the
        // real window through its tabs needs to find first.
        public static TabControl MainTabControl(Views.MainWindow window) =>
            window.GetVisualDescendants().OfType<TabControl>().First();
    }
}
