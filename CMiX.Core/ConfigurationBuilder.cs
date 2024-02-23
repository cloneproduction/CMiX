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
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;
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

            controlConfigurator.Register(new ModifierPairProfile());

            controlConfigurator.Register(new ManagerPairProfile());

            controlConfigurator.Register(new LightPairProfile());
            controlConfigurator.Register(new LightPairProfile());


            ///NEED TO REGISTER TWICE ????
            ///
            /////////CAMERA MODIFIERS
            controlConfigurator.Register(new CameraPairProfile());
            controlConfigurator.Register(new CameraPairProfile());

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
                    [typeof(EmptyPrefab)] = () => mapper.Map(new EmptyPrefabModel(), x.GetRequiredService<EmptyPrefab>()),
                    [typeof(Composition)] = () => mapper.Map(new CompositionModel(), x.GetRequiredService<Composition>()),
                    [typeof(Layer)] = () => mapper.Map(new LayerModel(), x.GetRequiredService<Layer>()),
                    [typeof(Entity)] = () => mapper.Map(new EntityModel(), x.GetRequiredService<Entity>()),
                    [typeof(LightEntity)] = () => mapper.Map(new LightEntityModel(), x.GetRequiredService<LightEntity>()),
                    [typeof(Texture)] = () => mapper.Map(new TextureModel(), x.GetRequiredService<Texture>()),
                    [typeof(Camera)] = () => mapper.Map(new CameraModel(), x.GetRequiredService<Camera>()),

                    [typeof(HSCB)] = () => mapper.Map(new HSCBModel(), x.GetRequiredService<HSCB>()),
                    [typeof(Blur)] = () => mapper.Map(new BlurModel(), x.GetRequiredService<Blur>()),
                    [typeof(RandomUV)] = () => mapper.Map(new RandomUVModel(), x.GetRequiredService<RandomUV>()),

                    [typeof(RandomHSV)] = () => mapper.Map(new RandomHSVModel(), x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRT)] = () => mapper.Map(new TransformSRTModel(), x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZ)] = () => mapper.Map(new RandomXYZModel(), x.GetRequiredService<RandomXYZ>()),

                    [typeof(Scale)] = () => mapper.Map(new ScaleModel(), x.GetRequiredService<Scale>()),
                    [typeof(Rotation)] = () => mapper.Map(new RotationModel(), x.GetRequiredService<Rotation>()),
                    [typeof(Translate)] = () => mapper.Map(new TranslateModel(), x.GetRequiredService<Translate>()),
                    [typeof(LinearXYZ)] = () => mapper.Map(new LinearXYZModel(), x.GetRequiredService<LinearXYZ>()),
                    [typeof(LFO)] = () => mapper.Map(new LFOModel(), x.GetRequiredService<LFO>()),
                    [typeof(RandomScale)] = () => mapper.Map(new RandomScaleModel(), x.GetRequiredService<RandomScale>()),
                    [typeof(Stepper)] = () => mapper.Map(new StepperModel(), x.GetRequiredService<Stepper>()),

                    [typeof(CameraLFO)] = () => mapper.Map(new CameraLFOModel(), x.GetRequiredService<CameraLFO>()),
                    [typeof(CameraRandom)] = () => mapper.Map(new CameraRandomModel(), x.GetRequiredService<CameraRandom>()),

                    [typeof(RandomPosition)] = () => mapper.Map(new RandomPositionModel(), x.GetRequiredService<RandomPosition>()),
                    [typeof(RandomRotation)] = () => mapper.Map(new RandomRotationModel(), x.GetRequiredService<RandomRotation>()),
            };

                Dictionary<Type, Func<IControlModel, IControl>> controlModelFactory = new Dictionary<Type, Func<IControlModel, IControl>>()
                {
                    [typeof(EmptyPrefabModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<EmptyPrefab>()),
                    [typeof(CompositionModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Composition>()),
                    [typeof(LayerModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Layer>()),
                    [typeof(EntityModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Entity>()),
                    [typeof(LightEntityModel)] = prefabModel => mapper.Map<IControlModel, IControl>(prefabModel, x.GetRequiredService<LightEntity>()),
                    [typeof(TextureModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Texture>()),
                    [typeof(CameraModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Camera>()),

                    [typeof(HSCBModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<HSCB>()),
                    [typeof(BlurModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Blur>()),
                    [typeof(RandomUVModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomUV>()),

                    [typeof(RandomHSVModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRTModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomXYZ>()),
                    [typeof(ScaleModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Scale>()),
                    [typeof(RotationModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Rotation>()),
                    [typeof(TranslateModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Translate>()),
                    [typeof(LinearXYZModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<LinearXYZ>()),
                    [typeof(LFOModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<LFO>()),
                    [typeof(RandomScaleModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomScale>()),
                    [typeof(StepperModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<Stepper>()),

                    [typeof(CameraLFOModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<CameraLFO>()),
                    [typeof(CameraRandomModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<CameraRandom>()),

                    [typeof(RandomPositionModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomPosition>()),
                    [typeof(RandomRotationModel)] = prefabModel => mapper.Map(prefabModel, x.GetRequiredService<RandomRotation>()),
                };

                return new ControlFactory(controlFactory, controlModelFactory, x.GetRequiredService<ControlRepository>());
            });

            var prefabConfigBuilder = new PrefabConfigurationBuilder(services);
        }
    }
}
