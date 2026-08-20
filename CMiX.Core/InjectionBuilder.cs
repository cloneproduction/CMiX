// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
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

            services.AddSingleton<Project>();
            services.AddSingleton<MasterBeat>();
            services.AddSingleton<Server>();

            services.AddSingleton<ControlActivationService>();
            services.AddSingleton<ControlFactory>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<MessageFactory>();
            services.AddSingleton<ControlRepository>();
            services.AddSingleton<AssetRepository>();
            services.AddSingleton<MessageCollectionManagerHandler>();
            services.AddSingleton<Client>();
        }

        public void ConfigureWpfTransport(IServiceProvider provider, Action<Action> dispatcher)
        {
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var server = provider.GetRequiredService<Server>();
            server.SetDispatcher(dispatcher);
            messenger.Register(server);
        }

        public void ConfigureVvvvTransport(IServiceProvider provider)
        {
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var client = provider.GetRequiredService<Client>();
            messenger.Register(client);
        }

        public void ConfigureVvvvServices(IServiceCollection services)
        {
            services.AddSingleton<ManagerReorderServiceFactory>(
                _ => (collection, onMove) => null);
        }
    }
}
