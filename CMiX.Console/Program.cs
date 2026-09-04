using System;
using System.ComponentModel;
using System.Threading;
using CMiX.Core.Compositing;
using CMiX.Core.DependencyInjection;
using CMiX.Core.Networking;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Console
{
    class Program
    {
        // How long the status line waits, so a replay of many changes does not fill the console.
        private static readonly TimeSpan PrintInterval = TimeSpan.FromMilliseconds(250);

        // The line of the last change that is not printed yet.
        private static string? _pendingLine;

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
            var printer = new Timer(_ => PrintPending(), null, PrintInterval, PrintInterval);

            if (System.Console.IsInputRedirected)
            {
                System.Console.WriteLine("Press Ctrl+C to exit.");
                using var exitEvent = new ManualResetEventSlim(false);
                System.Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    exitEvent.Set();
                };
                exitEvent.Wait();
            }
            else
            {
                System.Console.ReadLine();
            }

            peer.Stop();
            printer.Dispose();
            PrintPending();
            return 0;
        }

        // Keeps the line of the last change to a status property. The reader thread raises these.
        static void ReportChange(SyncPeer peer, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SyncPeer.Status) &&
                e.PropertyName != nameof(SyncPeer.LastAppliedId) &&
                e.PropertyName != nameof(SyncPeer.AppliedMessages) &&
                e.PropertyName != nameof(SyncPeer.ErrorMessage))
                return;

            Volatile.Write(ref _pendingLine,
                $"status={peer.Status} applied={peer.AppliedMessages} last={peer.LastAppliedId} tail={peer.TailId}");
        }

        // Prints the line of the last change, if there is one that is not printed yet.
        static void PrintPending()
        {
            var line = Interlocked.Exchange(ref _pendingLine, null);
            if (line != null)
                System.Console.WriteLine(line);
        }
    }
}
