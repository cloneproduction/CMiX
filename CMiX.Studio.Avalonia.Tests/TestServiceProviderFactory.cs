using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Tests
{
    // Mirrors the registrations in App.OnFrameworkInitializationCompleted so tests resolve
    // the same object graph the running app builds, minus the desktop lifetime plumbing.
    public static class TestServiceProviderFactory
    {
        public static IServiceProvider Create() => Create(_ => { });

        // The registrations of the app, then the replacements of one test.
        public static IServiceProvider Create(Action<IServiceCollection> replace)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            serviceCollection.AddCMiXCore();
            serviceCollection.AddSingleton<Views.MainWindow>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<MainWindowController>();
            serviceCollection.AddSingleton<MainMenuViewModel>();
            serviceCollection.AddSingleton<ProjectFileViewModel>();
            serviceCollection.AddSingleton<SettingsSyncViewModel>();
            serviceCollection.AddSingleton(ProjectSession.Create);
            serviceCollection.AddSingleton<UnsavedChanges>();
            serviceCollection.AddSingleton<FakeUnsavedChangesPrompt>();
            serviceCollection.AddSingleton<IUnsavedChangesPrompt>(provider => provider.GetRequiredService<FakeUnsavedChangesPrompt>());
            serviceCollection.AddSingleton<FakeMoveBlockedNotice>();
            serviceCollection.AddSingleton<IMoveBlockedNotice>(provider => provider.GetRequiredService<FakeMoveBlockedNotice>());
            serviceCollection.AddSingleton<FakeMessageNotice>();
            serviceCollection.AddSingleton<IMessageNotice>(provider => provider.GetRequiredService<FakeMessageNotice>());
            serviceCollection.AddSingleton<NotificationService>();
            serviceCollection.AddSingleton<INotificationService>(provider => provider.GetRequiredService<NotificationService>());
            serviceCollection.AddSingleton<PathDropService>();
            serviceCollection.AddSingleton<ProjectTree>();
            serviceCollection.AddSingleton<PanelFocusCycle>();
            // The defaults, with no file. A test never reads or writes the settings file of the Studio.
            serviceCollection.AddSingleton(_ => new InterfaceOptions());
            serviceCollection.AddSingleton(_ => new StudioSettingsFile(Path.Combine(Path.GetTempPath(), $"cmix-studiosettings-{Guid.NewGuid():N}", StudioSettings.FileName)));
            serviceCollection.AddSingleton<ICredentialStore, FakeCredentialStore>();
            // The defaults, with a file path in the temp folder. A test never reads or writes the file of the user.
            serviceCollection.AddSingleton(_ => new ShortcutMap(Path.Combine(Path.GetTempPath(), $"cmix-shortcuts-{Guid.NewGuid():N}", ShortcutMap.FileName)));
            // A test sets no mouse hook on the desktop.
            serviceCollection.AddSingleton<IDialogService>(provider => new DialogService(
                new MainWindowDialogManager(new EmptyViewLocator(), new DialogFactory()),
                viewModelFactory: type => provider.GetService(type)));
            serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                _ => (data, move) => new ManagerReorderService(data, move));
            replace(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        // Resolves and wires MainWindow the same way App does: DataContext assigned after
        // construction, then every control activated in one pass before anything is shown.
        public static Views.MainWindow CreateMainWindow(IServiceProvider provider)
        {
            var mainWindow = provider.GetRequiredService<Views.MainWindow>();

            // The notification hides on a clock that moves only by hand. A real timer would hide the notification during a later test.
            mainWindow.FindControl<CMiX.UI.Popups.PanelNotification>("panelNotification")!.TimeProvider = new CMiX.UI.Tests.ManualTimeProvider();
            mainWindow.DataContext = provider.GetRequiredService<MainViewModel>();
            provider.GetRequiredService<CMiX.Core.ControlActivationService>().ActivateAll();
            provider.GetRequiredService<UnsavedChanges>().MarkSaved();
            return mainWindow;
        }

        // The common starting point for a test that exercises the app through its real window:
        // build a provider, resolve and show MainWindow, and pump once so the initial layout and
        // binding pass settles before the test starts driving it.
        public static (IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel) ShowMainWindow() =>
            ShowMainWindow(_ => { });

        public static (IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel) ShowMainWindow(Action<IServiceCollection> replace)
        {
            var provider = Create(replace);
            var window = CreateMainWindow(provider);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (provider, window, provider.GetRequiredService<MainViewModel>());
        }

        // Two dispatcher passes, the idiom this suite uses wherever a UI visible change needs its
        // deferred bindings or layout to settle before assertions run.
        // New items start collapsed. A test that looks at the rows under a new item opens them first.
        public static void ExpandAll(CMiX.Studio.Avalonia.ViewModels.Tree.ProjectTree tree)
        {
            static void Open(CMiX.Studio.Avalonia.ViewModels.Tree.TreeNode node)
            {
                node.IsExpanded = true;
                foreach (var child in node.Children)
                    Open(child);
            }

            foreach (var root in tree.Roots)
                Open(root);
        }

        public static void Pump()
        {
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();
        }

        // The app has exactly one top level TabControl, the same one every test that drives the
        // real window through its tabs needs to find first.
        public static TabControl MainTabControl(Views.MainWindow window) =>
            window.GetVisualDescendants().OfType<TabControl>().First();

        // Shows the tab of the main window whose icon has this key, as the user does with a click on its header.
        public static TabItem SelectMainTab(Views.MainWindow window, string iconKey)
        {
            var tabs = MainTabControl(window);
            var tab = tabs.Items.OfType<TabItem>().Single(item => item.Header is CMiX.UI.Controls.AppIcon icon && icon.IconKey == iconKey);
            tabs.SelectedItem = tab;
            Pump();
            return tab;
        }
    }
}
