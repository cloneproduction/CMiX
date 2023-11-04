using System;
using System.Collections.Generic;
using System.Windows;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService<ServerManager>().AddNewServer(new Settings("127.0.0.1", 8080));
            var mainWindow = serviceProvider.GetRequiredService<Studio.Views.MainWindow> ();
            mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
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
            services.AddSingleton(x => new PrefabManagerBase(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00"), x.GetRequiredService<PrefabFactory>()));
        }
    }
}
