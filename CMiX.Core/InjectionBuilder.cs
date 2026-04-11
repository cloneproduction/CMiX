// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.DependencyInjection
{
    public class InjectionBuilder
    {
        public InjectionBuilder()
        {

        }
        public void ConfigureAllServices(IServiceCollection services)
        {
            services.Scan(selector => selector
                    .FromAssemblyOf<IControl>()
                    .AddClasses(classes => classes.AssignableTo<IControl>())
                    .AsSelf()
                    .WithTransientLifetime()
                );


            services.AddSingleton<UndoManager>();

            services.AddTransient<ControlFactory>();
            services.AddSingleton<Project>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<AppInitializer>();

            services.AddSingleton<MasterBeat>();

            services.AddSingleton<ControlActivationService>();
            services.AddSingleton<ControlFactory>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<MessageFactory>();
            services.AddSingleton<ControlRepository>();
            services.AddSingleton<AssetRepository>();
            services.AddSingleton<MessageCollectionManagerHandler>();
            services.AddSingleton<Client>();

            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IControl)
                                            && d.ImplementationType == typeof(PrefabManager));
        }
    }
}
