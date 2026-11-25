// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using CMiX.Core.Persistence;
using CMiX.Studio.Avalonia.Animations;
using CMiX.Studio.Avalonia.DependencyInjection;
using CMiX.Studio.Avalonia.Drag;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia
{
    public partial class App : Application
    {
        // The rule for ambient statics in this app, so the three that exist do not become a
        // habit. A static is acceptable only when the consumer has no injection point at all:
        // either XAML constructs it and its code behind cannot take constructor arguments
        // (MessageNotice, reached from AssetManager, and InteractionHost, reached from the value
        // editors), or it runs at process level after the
        // container is gone (_crashSaveProject, read by the unhandled exception handler).
        // Anything a container builds takes a constructor dependency instead, which is why the
        // view models, the managers and the messenger are all injected rather than reachable
        // here.
        private static Project? _crashSaveProject;
        public static IMessageNotice? MessageNotice { get; private set; }

        public override void Initialize()
        {
            StudioIcons.Register();
            AvaloniaXamlLoader.Load(this);
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
                serviceCollection.AddCMiXStudio(StudioSettings.DefaultPath, ShortcutMap.DefaultPath);

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                // The value editors open the drag of their value through the host.
                InteractionHostConnection.Connect(serviceProvider);
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
                serviceProvider.GetRequiredService<ControlRegistry>().ActivateAll();
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
