// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Services;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CMiX.Core.Prefabs;

namespace CMiX.Core
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider.GetRequiredService<Client>().Start(new Settings("127.0.0.1", 8080));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();
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

            services.AddSingleton(x => new PrefabFactory(new List<IPrefabFactory> { x.GetRequiredService<CompositionFactory>() }));
            services.AddSingleton(x => new PrefabManagerBase(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00"), x.GetRequiredService<PrefabFactory>()));
        }

        public ServiceProvider ServiceProvider { get; set; }
    }
}
