// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Animations;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Interaction;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia
{
    public partial class App : Application
    {
        // The rule for ambient statics in this app, so the four that exist do not become a
        // habit. A static is acceptable only when the consumer has no injection point at all:
        // either XAML constructs it and its code behind cannot take constructor arguments
        // (MessageNotice, reached from AssetManager, and InteractionHost, reached from the value
        // editors), or it runs at process level after the
        // container is gone (_crashSaveProject, read by the unhandled exception handler).
        // Anything a container builds takes a constructor dependency instead, which is why the
        // view models, the managers and the messenger are all injected rather than reachable
        // here. The one static outside both clauses is CMiX.Core.BaseControls.ValueInteraction,
        // whose whole purpose is being ambient across controls that do not know each other; it
        // is held to the rule by ownership instead, its BeginScope returns a handle and only
        // that handle can end the scope it opened. The editors reach it only through
        // InteractionHost, which App.Initialize connects to it.
        private static Project? _crashSaveProject;
        public static IMessageNotice? MessageNotice { get; private set; }

        public override void Initialize()
        {
            StudioIcons.Register();
            AvaloniaXamlLoader.Load(this);
            InteractionHost.BeginScopeHandler = () => ValueInteraction.BeginScope();
            // A press outside the open popups only closes them (see OutsideClick).
            CMiX.UI.Popups.OutsideClick.Install();
            // Registered last so explicit templates declared in views win over the reflection locator.
            DataTemplates.Add(new Views.ViewModelToViewTemplate());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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
                serviceCollection.AddSingleton<IUnsavedChangesPrompt, UnsavedChangesPrompt>();
                serviceCollection.AddSingleton<IMoveBlockedNotice, MoveBlockedNotice>();
                serviceCollection.AddSingleton<IMessageNotice, Notices.MessageNotice>();
                serviceCollection.AddSingleton<NotificationService>();
                serviceCollection.AddSingleton<INotificationService>(provider => provider.GetRequiredService<NotificationService>());
                serviceCollection.AddSingleton<PathDropService>();
                serviceCollection.AddSingleton<ProjectTree>();
                serviceCollection.AddSingleton<Views.PanelFocusCycle>();
                serviceCollection.AddSingleton(_ => new StudioSettingsFile(StudioSettings.DefaultPath));
                serviceCollection.AddSingleton<ICredentialStore, WindowsCredentialStore>();
                serviceCollection.AddSingleton(_ =>
                {
                    var options = new InterfaceOptions(StudioSettings.DefaultPath);
                    options.Load();
                    return options;
                });
                serviceCollection.AddSingleton(_ =>
                {
                    var shortcuts = new ShortcutMap();
                    shortcuts.Load();
                    return shortcuts;
                });
                serviceCollection.AddSingleton<IDialogService>(provider => new DialogService(
                    new MainWindowDialogManager(new EmptyViewLocator(), new DialogFactory()),
                    viewModelFactory: type => provider.GetService(type)));
                serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                    _ => (data, move) => new ManagerReorderService(data, move));

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                _crashSaveProject = serviceProvider.GetRequiredService<Project>();
                MessageNotice = serviceProvider.GetRequiredService<IMessageNotice>();
                var settingsFile = serviceProvider.GetRequiredService<StudioSettingsFile>();
                var credentialStore = serviceProvider.GetRequiredService<ICredentialStore>();
                // Post, not Invoke, so the Redis reader thread never waits for the UI thread.
                serviceProvider.GetRequiredService<SyncPeer>().StartAsStudio(
                    serviceProvider.GetRequiredService<ControlMessenger>(),
                    a => Dispatcher.UIThread.Post(a), settingsFile.Load().Resolve(credentialStore).Options);

                var masterBeat = serviceProvider.GetRequiredService<MasterBeat>();
                // The master beat holds the controller through its handlers, so no field keeps it.
                _ = new MasterBeatAnimationController(masterBeat);

                var mainWindow = serviceProvider.GetRequiredService<Views.MainWindow>();
                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
                serviceProvider.GetRequiredService<ControlActivationService>().ActivateAll();
                // The new session is the saved state. The window asks about unsaved changes against it.
                serviceProvider.GetRequiredService<UnsavedChanges>().MarkSaved();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        internal static void HandleUnhandledException(object exceptionObj)
        {
            try
            {
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "cmix-crash.txt"), $"{DateTime.Now}: {exceptionObj}\n");
            }
            catch
            {
                // Logging must never throw during crash handling.
            }

            EmergencySave();
        }

        private static void EmergencySave()
        {
            try
            {
                if (_crashSaveProject == null) return;

                var projectModel = ProjectModelBuilder.Build(_crashSaveProject);
                ProjectSerializer.Save(projectModel, Path.Combine(Path.GetTempPath(), "cmix-emergency.cmix"));
            }
            catch
            {
                // Emergency save must never throw during crash handling.
            }
        }
    }
}
