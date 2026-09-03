using CMiX.Core.Networking;

namespace CMiX.Console
{
    // Parses the command line arguments for the headless engine console.
    internal static class ConsoleArguments
    {
        public static void PrintUsage()
        {
            System.Console.WriteLine("Usage: CMiX.Console [options]");
            System.Console.WriteLine("  --ip <address>      Redis server address. Default 127.0.0.1.");
            System.Console.WriteLine("  --port <number>     Redis server port. Default 6379.");
            System.Console.WriteLine("  --db <number>       Redis database index. Default 0.");
            System.Console.WriteLine("  --user <name>       Redis user name. Default default.");
            System.Console.WriteLine("  --password <text>   Redis password. Default empty.");
            System.Console.WriteLine("  --prefix <text>     Key prefix for the store. Default cmix:default.");
            System.Console.WriteLine("  --name <text>       Peer name. Default is the machine name.");
            System.Console.WriteLine("  --help              Show this usage text.");
        }

        // Reads the arguments in pairs of flag and value. Returns null when an argument is
        // unknown or a value is missing or invalid. The caller then exits with code 2.
        public static SyncOptions? Parse(string[] args)
        {
            var ip = "";
            var port = 0;
            var database = 0;
            var user = "";
            var password = "";
            var prefix = "";
            var name = "";

            for (var i = 0; i < args.Length; i += 2)
            {
                var flag = args[i];
                if (i + 1 >= args.Length)
                {
                    PrintMissingValue(flag);
                    return null;
                }

                var value = args[i + 1];
                switch (flag)
                {
                    case "--ip":
                        ip = value;
                        break;
                    case "--port":
                        if (!int.TryParse(value, out port)) { PrintUnknown(flag); return null; }
                        break;
                    case "--db":
                        if (!int.TryParse(value, out database)) { PrintUnknown(flag); return null; }
                        break;
                    case "--user":
                        user = value;
                        break;
                    case "--password":
                        password = value;
                        break;
                    case "--prefix":
                        prefix = value;
                        break;
                    case "--name":
                        name = value;
                        break;
                    default:
                        PrintUnknown(flag);
                        return null;
                }
            }

            if (string.IsNullOrEmpty(name))
                name = System.Environment.MachineName;

            return SyncOptions.Create(ip, port, database, user, password, prefix, name);
        }

        private static void PrintUnknown(string arg)
        {
            System.Console.WriteLine($"Unknown argument: {arg}");
            PrintUsage();
        }

        private static void PrintMissingValue(string flag)
        {
            System.Console.WriteLine($"Missing value for {flag}");
            PrintUsage();
        }
    }
}
