using System;
using System.Windows;
using CMiX.Core;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CMiX
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InjectionBuilder configurationBuilder = new InjectionBuilder();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(); // this is necessary since update to automapper 16.0.0

            configurationBuilder.ConfigureServices(serviceCollection);

            this.ConfigureUIService(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<Studio.Views.MainWindow>();

            mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();

            mainWindow.Show();
        }

        private void ConfigureUIService(IServiceCollection services)
        {
            services.AddSingleton<Studio.Views.MainWindow>();
        }
    }
}
