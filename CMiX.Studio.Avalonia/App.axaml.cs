// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Animations;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia
{
    public partial class App : Application
    {
        private MasterBeatAnimationController _animationController;
        private static Project _crashSaveProject;

        // Set once at startup so XAML instantiated views without constructor
        // injection, such as ServerCreation, can reach the shared dialog service.
        public static IDialogService DialogService { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Themes.Icons.Register(Resources);
            // Registered last so explicit templates declared in views win over the reflection locator.
            DataTemplates.Add(new Views.ViewModelToViewTemplate());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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
                    new MainWindowDialogManager(new DialogViewLocator(), new DialogFactory().AddMessageBox()),
                    viewModelFactory: type => provider.GetService(type)));
                serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                    _ => (collection, onMove) => new ManagerReorderService(collection, onMove));

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                _crashSaveProject = serviceProvider.GetRequiredService<Project>();
                DialogService = serviceProvider.GetRequiredService<IDialogService>();
                // Post rather than Invoke, so the WatsonTcp receive thread never blocks on the
                // UI thread being free.
                configurationBuilder.ConfigureWpfTransport(serviceProvider,
                    a => Dispatcher.UIThread.Post(a));

                var masterBeat = serviceProvider.GetRequiredService<MasterBeat>();
                _animationController = new MasterBeatAnimationController(masterBeat);

                var mainWindow = serviceProvider.GetRequiredService<Views.MainWindow>();
                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
                serviceProvider.GetRequiredService<ControlActivationService>().ActivateAll();
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
                if (_crashSaveProject?.CompositionManager?.SelectedItem is not Composition selectedComposition) return;

                var compositionModel = (CompositionModel)selectedComposition.ToModel();
                var projectModel = new ProjectModel
                {
                    MasterBeat = (MasterBeatModel)_crashSaveProject.MasterBeat.ToModel(),
                    CompositionManager = new PrefabManagerModel
                    {
                        ManagerData = new ManagerDataModel
                        {
                            Items = new Collection<IControlModel> { compositionModel },
                            SelectedIndex = 0
                        }
                    }
                };
                ProjectSerializer.Save(projectModel, Path.Combine(Path.GetTempPath(), "cmix-emergency.cmix"));
            }
            catch
            {
                // Emergency save must never throw during crash handling.
            }
        }
    }
}
