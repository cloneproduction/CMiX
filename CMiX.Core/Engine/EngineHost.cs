// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Engine
{
    // The engine side of the sync for a vvvv patch. Start builds the services and starts the engine peer.
    // Dispose stops the peer and closes the queue. The patch drains the queue once per frame.
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

        public IServiceProvider ServiceProvider => _running?.Provider;

        public MainloopQueue MainloopQueue => _running?.Queue;

        public void Start(SyncOptions options, int readBatch = 256)
        {
            if (_running != null)
                throw new InvalidOperationException("The engine host is already started.");

            var builder = new InjectionBuilder();
            var services = new ServiceCollection();
            builder.ConfigureAllServices(services);
            _overrideServices(services);

            var provider = services.BuildServiceProvider();
            var queue = new MainloopQueue();
            builder.ConfigureEngineTransport(provider, options, queue, readBatch);
            _running = new Running(provider, queue);
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

        private sealed record Running(IServiceProvider Provider, MainloopQueue Queue);
    }
}
