// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;
using VL.Core.Import;

namespace CMiX.Core.Engine
{
    // The engine side of the sync as one vvvv node. It builds the services and starts the engine peer
    // on the first frame. Dispose stops the peer and closes the queue. The patch drains the queue once per frame.
    [ProcessNode]
    public sealed class EngineHost : IDisposable
    {
        private readonly Action<IServiceCollection> _overrideServices;
        private Running _running;

        public EngineHost() : this(_ => { })
        {
        }

        // The tests use this to replace services, such as the store of the peer.
        internal EngineHost(Action<IServiceCollection> overrideServices)
        {
            _overrideServices = overrideServices;
        }

        // The options and the read batch of the first call are used. Later calls change nothing.
        public void Update(out IServiceProvider serviceProvider, out MainloopQueue mainloopQueue, SyncOptions options, int readBatch = 256)
        {
            _running ??= Start(options, readBatch);
            serviceProvider = _running.Provider;
            mainloopQueue = _running.Queue;
        }

        public void Dispose()
        {
            var running = _running;
            _running = null;
            if (running == null)
                return;

            running.Provider.GetRequiredService<SyncPeer>().Stop();
            running.Queue.Close();
        }

        private Running Start(SyncOptions options, int readBatch)
        {
            var builder = new InjectionBuilder();
            var services = new ServiceCollection();
            builder.ConfigureAllServices(services);
            builder.ConfigureVvvvServices(services);
            _overrideServices(services);

            var provider = services.BuildServiceProvider();
            var queue = new MainloopQueue();
            builder.ConfigureEngineTransport(provider, options, queue, readBatch);
            return new Running(provider, queue);
        }

        private sealed record Running(IServiceProvider Provider, MainloopQueue Queue);
    }
}
