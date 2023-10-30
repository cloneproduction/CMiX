using System;
using System.Collections.Generic;
using System.Windows;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using CMiX.Studio.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;

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

            var mainWindow = ServiceProvider.GetRequiredService<Studio.Views.MainWindow> ();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDialogFactory, DialogFactory>();
            services.AddSingleton<Studio.Views.MainWindow>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<Project>();
            services.AddSingleton<PrefabRepositories>();
            services.AddSingleton<MasterBeat>();
            services.AddSingleton<CompositionFactory>();
            services.AddSingleton<AssetManager>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();
            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ServerFactory>();

            services.AddSingleton(x => new PrefabFactory(new List<IPrefabFactory> {x.GetRequiredService<CompositionFactory>()}));
            services.AddSingleton(x => new PrefabManagerBase(Guid.Parse("00000000-0000-0000-0000-000000000001"), x.GetRequiredService<PrefabFactory>()));
        }
    }
}
