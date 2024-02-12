// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Mapping;
using CMiX.Core.Modifiers;
using CMiX.Core.Network;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Transformation;
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

        }

        public IServiceCollection ServiceProvider { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CerasSerializer>();
            services.AddSingleton<MainViewModel>();
            
            services.AddSingleton<AssetManager>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<Client>();

            services.AddSingleton<ServerFactory>();
            services.AddSingleton<ServerManager>();
            services.AddSingleton<ServerRepository>();

            ////////
            services.AddSingleton<MessageProcessor>();
            services.AddSingleton<ControlMessenger>();



            MapperConfigurationExpression mapperConfigurationExpression = new MapperConfigurationExpression();

            services.AddTransient(x =>
            {
                mapperConfigurationExpression.AddProfile(new BaseControlMappingProfile());
                mapperConfigurationExpression.AddProfile(new PrefabMappingProfile());
                mapperConfigurationExpression.AddProfile(new MasterBeatProfile());

                var config = new MapperConfiguration(mapperConfigurationExpression);
                return config.CreateMapper();
            });

            services.AddSingleton<MasterBeat>();


            var controlConfigurator = new ControlConfigurator(mapperConfigurationExpression, services);

            controlConfigurator.Register(new BaseControlPairProfile());
            controlConfigurator.Register(new TransformPairProfile());
            controlConfigurator.Register(new BeatPairProfile());
            controlConfigurator.Register(new TexturePairProfile());
            controlConfigurator.Register(new TexturingPairProfile());
            controlConfigurator.Register(new AnimationPairProfile());
            controlConfigurator.Register(new MaterialPairProfile());
            controlConfigurator.Register(new MaskPairProfile());
            controlConfigurator.Register(new MeshPairProfile());
            controlConfigurator.Register(new RenderingPairProfile());
            controlConfigurator.Register(new CameraPairProfile());
            controlConfigurator.Register(new ModifierPairProfile());

            controlConfigurator.Register(new ManagerPairProfile());


            ///NEED TO REGISTER TWICE ????
            ///
            /////////ENTITY MODIFIERS
            var entityModifiers = new EntityModifierPairProfile();
            controlConfigurator.Register(entityModifiers);
            controlConfigurator.Register(entityModifiers);


            /////////TEXTURE MODIFIERS
            var textureModifiers = new TextureModifierPairProfile();
            controlConfigurator.Register(textureModifiers);
            controlConfigurator.Register(textureModifiers);


            services.AddSingleton(x =>
            {
                var mapper = x.GetRequiredService<IMapper>();

                Dictionary<Type, Func<IControl>> controlFactory = new Dictionary<Type, Func<IControl>>()
                {
                    [typeof(EmptyPrefab)] = () => mapper.Map<IControlModel, IControl>(new EmptyPrefabModel(), x.GetRequiredService<EmptyPrefab>()),
                    [typeof(Composition)] = () => mapper.Map<IControlModel, IControl>(new CompositionModel(), x.GetRequiredService<Composition>()),
                    [typeof(Layer)] = () => mapper.Map<IControlModel, IControl>(new LayerModel(), x.GetRequiredService<Layer>()),
                    [typeof(Entity)] = () => mapper.Map<IControlModel, IControl>(new EntityModel(), x.GetRequiredService<Entity>()),
                    [typeof(LightEntity)] = () => mapper.Map<IControlModel, IControl>(new LightEntityModel(), x.GetRequiredService<LightEntity>()),
                    [typeof(Texture)] = () => mapper.Map<IControlModel, IControl>(new TextureModel(), x.GetRequiredService<Texture>()),

                    [typeof(HSCB)] = () => mapper.Map<IControlModel, IControl>(new HSCBModel(), x.GetRequiredService<HSCB>()),
                    [typeof(Blur)] = () => mapper.Map<IControlModel, IControl>(new BlurModel(), x.GetRequiredService<Blur>()),

                    [typeof(RandomHSV)] = () => mapper.Map<IControlModel, IControl>(new RandomHSVModel(), x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRT)] = () => mapper.Map<IControlModel, IControl>(new TransformSRTModel(), x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZ)] = () => mapper.Map<IControlModel, IControl>(new RandomXYZModel(), x.GetRequiredService<RandomXYZ>()),
                };


                Dictionary<Type, Func<IControlModel, IControl>> controlModelFactory = new Dictionary<Type, Func<IControlModel, IControl>>()
                {
                    [typeof(EmptyPrefabModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<EmptyPrefab>()),
                    [typeof(CompositionModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<Composition>()),
                    [typeof(LayerModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<Layer>()),
                    [typeof(EntityModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<Entity>()),
                    [typeof(LightEntityModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<LightEntity>()),
                    [typeof(TextureModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<Texture>()),

                    [typeof(HSCBModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<HSCB>()),
                    [typeof(BlurModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<Blur>()),

                    [typeof(RandomHSVModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRTModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<RandomXYZ>()),
                };

                return new ControlFactory(controlFactory, controlModelFactory, x.GetRequiredService<ControlRepository>());
            });

            var prefabConfigBuilder = new PrefabConfigurationBuilder(services);
        }
    }
}
