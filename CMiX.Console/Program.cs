using CMiX.Core;
using CMiX.Core.Compositing;

namespace CMiX.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            var project = configurationBuilder.ServiceProvider.GetService(typeof(Project));

            System.Console.ReadLine();
        }
    }
}
