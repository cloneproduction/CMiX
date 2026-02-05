// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Reflection;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Mapping;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class InjectionBuilder
    {
        public InjectionBuilder()
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Scan(selector => selector
                    .FromCallingAssembly()
                    .AddClasses(classes => classes.AssignableTo<IControl>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime()
            );

            services.AddSingleton<Project>();

            services.AddSingleton(sp =>
            {
                var config = new SerializerConfig();

                CerasMessageConfiguration.Configure(config);

                return new CerasSerializer(config);
            });


            services.AddSingleton<MainViewModel>();

            services.AddSingleton<ControlRepository>();
            services.AddSingleton<ControlFactory>();
            services.AddSingleton<AssetRepository>();

            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();

            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<MessageFactory>();
            services.AddSingleton<MessageSerializer>();
            services.AddSingleton<MasterBeat>();

            services.AddAutoMapper((provider, opt) =>
            {
                opt.AddMaps("CMiX.Core");
                opt.ConstructServicesUsing(t => ActivatorUtilities.CreateInstance(provider, t));
            }, Assembly.GetAssembly(typeof(ControlsProfile)));
        }
    }
}
