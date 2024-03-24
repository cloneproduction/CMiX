// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Reflection;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Mapping;
using CMiX.Core.Network;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using CMiX.Core.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder()
        {

        }

        public IServiceCollection ServiceProvider { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Scan(selector => selector
                    .FromCallingAssembly()
                    .AddClasses(classes => classes.AssignableTo<IControl>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime()
            );

            services.AddSingleton<Project>();
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<ControlRepository>();
            services.AddSingleton<AssetManager>();

            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();

            services.AddSingleton<ServerFactory>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<ServerRepository>();

            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ControlMessenger>();
            services.AddSingleton<EventMessenger>();
            services.AddSingleton<ManagerMessenger>();
            services.AddSingleton<MainMenuMessenger>();

            services.AddSingleton<MasterBeat>();
            services.AddSingleton<ControlFactory>();

            services.AddAutoMapper((provider, opt) =>
            {
                opt.AddMaps("CMiX.Core");
                opt.ConstructServicesUsing(t => ActivatorUtilities.CreateInstance(provider, t));
            }, Assembly.GetAssembly(typeof(PrefabMappingProfile)));



            //Mapper.Initialize(m =>
            //{
            //    m.ConstructServicesUsing(container.Resolve);

            //    m.CreateMap<Test3, ITest>().ConstructUsingServiceLocator(); // This is important!

            //});



            //services.AddAutoMapper(cfg =>
            //{
            //    cfg.AddMaps("CMiX.Core");
            //    //cfg.ConstructServicesUsing(services)

            //}, typeof(PrefabMappingProfile).Assembly);

            //, cfg.ConstructServicesUsing);
            //, 
            //services.AddAutoMapper(typeof(PrefabMappingProfile).Assembly);
            //services.AddAutoMapper();

            //MapperConfigurationExpression mapperConfigurationExpression = new MapperConfigurationExpression();

            //services.AddTransient(x =>
            //{
            //    mapperConfigurationExpression.AddMaps("CMiX.Core");
            //    mapperConfigurationExpression.ConstructServicesUsing(t => x.GetR(t));
            //    mapperConfigurationExpression.AllowNullCollections = true;
            //    mapperConfigurationExpression.AllowNullDestinationValues = true;
            //    mapperConfigurationExpression.AddCollectionMappers();

            //    var config = new MapperConfiguration(mapperConfigurationExpression);

            //    //config.AssertConfigurationIsValid();
            //    return config.CreateMapper();
            //});


            //var modifierTypes = Assembly.GetExecutingAssembly()
            //                .GetTypes()
            //                .Where(type => typeof(IModifier).IsAssignableFrom(type) && !type.IsInterface);


            //foreach (var type in modifierTypes)
            //{

            //    var t = Type.GetType(modifierTypes + "Model");
            //    Console.WriteLine("POUETPOUET" + t);
            //}
        }
    }
}
