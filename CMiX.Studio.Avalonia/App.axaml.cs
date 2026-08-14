// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
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
                    new DialogManager(viewLocator: new DialogViewLocator()),
                    viewModelFactory: type => provider.GetService(type)));
                serviceCollection.AddSingleton<ManagerReorderServiceFactory>(
                    _ => (collection, onMove) => new ManagerReorderService(collection, onMove));

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                configurationBuilder.ConfigureWpfTransport(serviceProvider,
                    a => Dispatcher.UIThread.Invoke(a));

                var mainWindow = serviceProvider.GetRequiredService<Views.MainWindow>();
                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
                serviceProvider.GetRequiredService<ControlActivationService>().ActivateAll();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
