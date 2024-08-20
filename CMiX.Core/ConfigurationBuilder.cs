// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Reflection;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Mapping;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using CMiX.Core.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder()
        {

        }

        public IServiceCollection ServiceProvider { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Scan(selector => selector
                    .FromCallingAssembly()
                    .AddClasses(classes => classes.AssignableTo<IControl>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime()
            );


            //services.AddTransient<OutputSettings>(x => new OutputSettings(new Integer2(new GenericValue<int>(888, x.GetService<ControlMessenger>()), new GenericValue<int>(666, x.GetService<ControlMessenger>())), x.GetService<GenericValue<string>>()));

            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<ControlRepository>();
            services.AddSingleton<ControlFactory>();

            services.AddSingleton<AssetManager>();

            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();

            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<EventMessenger>();
            services.AddSingleton<ManagerMessenger>();

            services.AddSingleton<MainMenuMessenger>();

            services.AddSingleton<MasterBeat>();


            services.AddAutoMapper((provider, opt) =>
            {
                opt.AddMaps("CMiX.Core");
                opt.ConstructServicesUsing(t => ActivatorUtilities.CreateInstance(provider, t));
            }, Assembly.GetAssembly(typeof(PrefabMappingProfile)));
        }
    }
}
