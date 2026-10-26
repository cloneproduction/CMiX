// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Processors;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.DependencyInjection
{
    public static class CMiXServiceCollectionExtensions
    {
        // Registers the services that every host needs: Studio, engine and Console.
        public static IServiceCollection AddCMiXCore(this IServiceCollection services)
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
            services.AddSingleton<RedisAddressRegistry>();

            services.AddSingleton<ControlActivationService>();
            services.AddSingleton<ControlFactory>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<MessageFactory>();
            services.AddSingleton<ControlRepository>();
            services.AddSingleton<VisibilityProcessor>();
            services.AddSingleton<UpdateOrderProcessor>();
            services.AddSingleton<ActiveModifierProcessor>();
            services.AddSingleton<VisibleItemsProcessor>();
            services.AddSingleton<AssetRepository>();
            services.AddSingleton<MessageCollectionManagerHandler>();
            services.AddTransient<ManagerData>();
            services.AddTransient<CollectionManager>();

            services.AddSingleton<ISyncTarget, ProjectSyncTarget>();
            services.AddSingleton<Func<SyncOptions, ISyncStore>>(_ => options => new RedisSyncStore(options));
            services.AddSingleton<SyncPeer>();

            return services;
        }
    }
}
