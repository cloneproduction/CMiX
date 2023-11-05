// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Mapping;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Network;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class ConfigurationBuilder
    {
        public ConfigurationBuilder()
        {
            //ServiceCollection serviceCollection = new ServiceCollection();
            //ConfigureServices(serviceCollection);
            //ServiceProvider = serviceCollection.BuildServiceProvider();
            //ServiceProvider.GetRequiredService<Client>().Start(new Settings("127.0.0.1", 8080));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(x =>
            {
                var mappingProfile = new MappingProfile();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MappingProfile());

                    foreach (var profile in mappingProfile.Profiles)
                    {
                        cfg.AddProfile(profile);
                    }
                });
                return config.CreateMapper();
            });


            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<Project>();
            services.AddSingleton<PrefabRepositories>();
            services.AddSingleton<MasterBeat>();
            services.AddSingleton<CompositionFactory>();
            services.AddSingleton<AssetManager>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();
            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ServerFactory>();

            ////////
            services.AddSingleton<ManagerMessenger>();
            services.AddSingleton<ControlMessenger>();

            ////////
            services.AddTransient<BooleanValue>();
            services.AddTransient<FloatValue>();
            services.AddTransient<StringValue>();
            services.AddTransient<IntegerValue>();
            services.AddTransient<Button>();
            services.AddTransient<Integer2>();
            services.AddTransient<ColorValue>();


            services.AddTransient<PrefabService>();
            services.AddTransient<OutputSettings>();

            services.AddSingleton<TextureModifierFactory>();

            services.AddTransient<Composition>(x =>
                new Composition(x.GetRequiredService<PrefabService>(), x.GetRequiredService<MasterBeat>(), x.GetRequiredService<PrefabManager>(), new ModifierManager(x.GetRequiredService<TextureModifierFactory>(), x.GetRequiredService<ManagerMessenger>()), x.GetRequiredService<OutputSettings>()));
            
            services.AddTransient<Layer>();

            services.AddTransient<Entity>(x => 
                new Entity(x.GetRequiredService<PrefabService>(), x.GetRequiredService<Mesh>(), x.GetRequiredService<Material>(), new ModifierManager(new EntityModifierFactory(), x.GetRequiredService<ManagerMessenger>())));

            services.AddTransient<PrefabManagerSlot>();
            services.AddTransient<PrefabManager>();

            services.AddTransient<Mesh>();
            services.AddTransient<Material>();

            //services.AddSingleton(x => new PrefabFactory(new List<IPrefabFactory> { x.GetRequiredService<CompositionFactory>() }));
            services.AddSingleton(x => new PrefabManagerBase(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00"), x.GetRequiredService<PrefabFactory>(), x.GetRequiredService<ManagerMessenger>()));

            services.AddSingleton<PrefabFactory>(ctx =>
            {
                var factories = new Dictionary<Type, Func<IPrefab>>()
                {
                    [typeof(Composition)] = () => ctx.GetRequiredService<Composition>(),
                    [typeof(CompositionModel)] = () => ctx.GetRequiredService<Composition>(),
                    [typeof(Layer)] = () => ctx.GetRequiredService<Layer>(),
                    [typeof(LayerModel)] = () => ctx.GetRequiredService<Layer>(),
                    [typeof(Entity)] = () => ctx.GetRequiredService<Entity>(),
                    [typeof(EntityModel)] = () => ctx.GetRequiredService<Entity>(),
                    [typeof(LightEntity)] = () => ctx.GetRequiredService<LightEntity>(),
                    [typeof(LightEntityModel)] = () => ctx.GetRequiredService<LightEntity>(),
                    [typeof(Texture)] = () => ctx.GetRequiredService<Texture>(),
                    [typeof(TextureModel)] = () => ctx.GetRequiredService<Texture>()
                };

                return new PrefabFactory(factories, ctx.GetRequiredService<PrefabRepositories>());
            });
        }

        public ServiceProvider ServiceProvider { get; set; }
    }
}
