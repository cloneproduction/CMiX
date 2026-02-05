using System;
using System.Configuration;
using System.Windows;
using CMiX.Core;
using CMiX.Core.Mapping;
using CMiX.Core.ViewModels;
using CMiX.Studio.Views.Modifiers;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
           /* var xamlDictionary = new ResourceDictionary
            {
                Source = new Uri("/CMiX.Studio;component/Views/Modifiers.xaml", UriKind.Relative)
            };

            // 2️⃣ Generate dynamic DataTemplates
            var textureFilterProfile = new TextureFilterProfile();
            ModifierDataTemplateGenerator.GenerateTemplates(xamlDictionary, textureFilterProfile.RegisteredTypes);

            // 3️⃣ Merge the XAML + dynamic templates into Application resources
            Application.Current.Resources.MergedDictionaries.Add(xamlDictionary);*/



            InjectionBuilder configurationBuilder = new InjectionBuilder();

            var serviceCollection = new ServiceCollection();
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
