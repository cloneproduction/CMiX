// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
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

            services.AddSingleton<ControlActivationService>();
            services.AddSingleton<ControlFactory>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<MessageFactory>();
            services.AddSingleton<ControlRepository>();
            services.AddSingleton<AssetRepository>();
            services.AddSingleton<MessageCollectionManagerHandler>();

            services.AddSingleton<ISyncTarget, ProjectSyncTarget>();
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => options => new RedisSyncStore(options));
            services.AddSingleton<SyncPeer>();
        }

        public void ConfigureVvvvServices(IServiceCollection services)
        {
            services.AddSingleton<ManagerReorderServiceFactory>(
                _ => (collection, onMove) => null);
        }

        public void ConfigureStudioTransport(IServiceProvider provider, Action<Action> dispatcher, SyncOptions options)
        {
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var peer = provider.GetRequiredService<SyncPeer>();
            peer.SetDispatcher(dispatcher);
            messenger.Register(peer);
            peer.CompactionEnabled = true;
            peer.IsWriter = true;
            peer.ListPeersEnabled = true;
            peer.Start(options with { Role = "studio" }, autoJoin: false);
        }

        public void ConfigureEngineTransport(IServiceProvider provider, SyncOptions options)
        {
            var peer = provider.GetRequiredService<SyncPeer>();
            // Engines read and apply. Only the Studio writes, so the engine peer is not a sender.
            peer.Start(options, autoJoin: true);
        }
    }
}
