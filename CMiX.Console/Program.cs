using System;
using System.ComponentModel;
using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            if (Array.Exists(args, a => a == "--help"))
            {
                ConsoleArguments.PrintUsage();
                return 0;
            }

            var options = ConsoleArguments.Parse(args);
            if (options == null)
                return 2;

            System.Console.WriteLine(
                $"ip={options.Ip} port={options.Port} db={options.Database} prefix={options.KeyPrefix} name={options.PeerName}");

            InjectionBuilder configurationBuilder = new InjectionBuilder();

            ServiceCollection serviceCollection = new ServiceCollection();
            configurationBuilder.ConfigureAllServices(serviceCollection);
            configurationBuilder.ConfigureVvvvServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<Project>();
            configurationBuilder.ConfigureEngineTransport(serviceProvider, options);

            var peer = serviceProvider.GetRequiredService<SyncPeer>();
            peer.PropertyChanged += (sender, e) => ReportChange(peer, e);

            System.Console.ReadLine();
            peer.Stop();
            return 0;
        }

        // Prints one line per change to a status property. The reader thread raises these.
        static void ReportChange(SyncPeer peer, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SyncPeer.Status) &&
                e.PropertyName != nameof(SyncPeer.LastAppliedId) &&
                e.PropertyName != nameof(SyncPeer.AppliedMessages) &&
                e.PropertyName != nameof(SyncPeer.ErrorMessage))
                return;

            System.Console.WriteLine(
                $"status={peer.Status} applied={peer.AppliedMessages} last={peer.LastAppliedId} tail={peer.TailId}");
        }
    }
}
