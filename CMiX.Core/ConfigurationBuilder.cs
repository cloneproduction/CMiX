// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using Ceras;
using CMiX.Core.Animations;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Network;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
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
            services.Scan(selector => selector
                    .FromCallingAssembly()
                    .AddClasses(classes => classes.AssignableTo<IControl>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime()
            );

            services.AddSingleton(x => new Project(new PrefabManager(new ManagerData(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00")), x.GetRequiredService<ControlFactory>(), x.GetRequiredService<ManagerMessenger>())));

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

            services.AddSingleton<MasterBeat>();


            MapperConfigurationExpression mapperConfigurationExpression = new MapperConfigurationExpression();

            services.AddTransient(x =>
            {
                mapperConfigurationExpression.AddMaps("CMiX.Core");

                var config = new MapperConfiguration(mapperConfigurationExpression);
                return config.CreateMapper();
            });


            //var modifierTypes = Assembly.GetExecutingAssembly()
            //                .GetTypes()
            //                .Where(type => typeof(IModifier).IsAssignableFrom(type) && !type.IsInterface);


            //foreach (var type in modifierTypes)
            //{

            //    var t = Type.GetType(modifierTypes + "Model");
            //    Console.WriteLine("POUETPOUET" + t);
            //}

            services.AddSingleton(x =>
            {
                var mapper = x.GetRequiredService<IMapper>();

                var prefabFactory = new Dictionary<Type, Func<IControl>>()
                {
                    ////PREFABS
                    [typeof(EmptyPrefab)] = () => mapper.Map(new EmptyPrefabModel(), x.GetRequiredService<EmptyPrefab>()),
                    [typeof(Composition)] = () => mapper.Map(new CompositionModel(), x.GetRequiredService<Composition>()),
                    [typeof(Layer)] = () => mapper.Map(new LayerModel(), x.GetRequiredService<Layer>()),
                    [typeof(Entity)] = () => mapper.Map(new EntityModel(), x.GetRequiredService<Entity>()),
                    [typeof(LightEntity)] = () => mapper.Map(new LightEntityModel(), x.GetRequiredService<LightEntity>()),
                    [typeof(Texture)] = () => mapper.Map(new TextureModel(), x.GetRequiredService<Texture>()),
                    [typeof(Camera)] = () => mapper.Map(new CameraModel(), x.GetRequiredService<Camera>()),
                };

                var filterFactory = new Dictionary<Type, Func<IControl>>()
                {
                    ////TEXTURE FILTERS
                    [typeof(HSCB)] = () => mapper.Map(new HSCBModel(), x.GetRequiredService<HSCB>()),
                    [typeof(Invert)] = () => mapper.Map(new InvertModel(), x.GetRequiredService<Invert>()),
                    [typeof(Edge)] = () => mapper.Map(new EdgeModel(), x.GetRequiredService<Edge>()),
                    [typeof(Blur)] = () => mapper.Map(new BlurModel(), x.GetRequiredService<Blur>()),
                    [typeof(RandomUV)] = () => mapper.Map(new RandomUVModel(), x.GetRequiredService<RandomUV>()),
                    [typeof(Feedback)] = () => mapper.Map(new FeedbackModel(), x.GetRequiredService<Feedback>()),
                    [typeof(Pixelate)] = () => mapper.Map(new PixelateModel(), x.GetRequiredService<Pixelate>()),
                    [typeof(LFOUV)] = () => mapper.Map(new LFOUVModel(), x.GetRequiredService<LFOUV>()),
                    [typeof(Echo)] = () => mapper.Map(new EchoModel(), x.GetRequiredService<Echo>()),
                    [typeof(TransformTexture)] = () => mapper.Map(new TransformTextureModel(), x.GetRequiredService<TransformTexture>()),
                };


                var controlFactory = new Dictionary<Type, Func<IControl>>()
                {
                    [typeof(RandomHSV)] = () => mapper.Map(new RandomHSVModel(), x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRT)] = () => mapper.Map(new TransformSRTModel(), x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZ)] = () => mapper.Map(new RandomXYZModel(), x.GetRequiredService<RandomXYZ>()),
                    [typeof(GaussianXYZ)] = () => mapper.Map(new GaussianXYZModel(), x.GetRequiredService<GaussianXYZ>()),

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

                var finalDictionary = controlFactory.Concat(prefabFactory).Concat(filterFactory).ToDictionary(e => e.Key, e => e.Value);



                var controlModelFactory = new Dictionary<Type, Func<IControlModel, IControl>>()
                {
                    
                    ////PREFABS
                    [typeof(EmptyPrefabModel)] = model => mapper.Map(model, x.GetRequiredService<EmptyPrefab>()),
                    [typeof(CompositionModel)] = model => mapper.Map(model, x.GetRequiredService<Composition>()),
                    [typeof(LayerModel)] = model => mapper.Map(model, x.GetRequiredService<Layer>()),
                    [typeof(EntityModel)] = model => mapper.Map(model, x.GetRequiredService<Entity>()),
                    [typeof(LightEntityModel)] = model => mapper.Map(model, x.GetRequiredService<LightEntity>()),
                    [typeof(TextureModel)] = model => mapper.Map(model, x.GetRequiredService<Texture>()),
                    [typeof(CameraModel)] = model => mapper.Map(model, x.GetRequiredService<Camera>()),

                    ////TEXTURE FILTERS
                    [typeof(HSCBModel)] = model => mapper.Map(model, x.GetRequiredService<HSCB>()),
                    [typeof(InvertModel)] = model => mapper.Map(model, x.GetRequiredService<Invert>()),
                    [typeof(EdgeModel)] = model => mapper.Map(model, x.GetRequiredService<Edge>()),
                    [typeof(BlurModel)] = model => mapper.Map(model, x.GetRequiredService<Blur>()),
                    [typeof(RandomUVModel)] = model => mapper.Map(model, x.GetRequiredService<RandomUV>()),
                    [typeof(FeedbackModel)] = model => mapper.Map(model, x.GetRequiredService<Feedback>()),
                    [typeof(PixelateModel)] = model => mapper.Map(model, x.GetRequiredService<Pixelate>()),
                    [typeof(LFOUVModel)] = model => mapper.Map(model, x.GetRequiredService<LFOUV>()),
                    [typeof(EchoModel)] = model => mapper.Map(model, x.GetRequiredService<Echo>()),
                    [typeof(TransformTextureModel)] = model => mapper.Map(model, x.GetRequiredService<TransformTexture>()),


                    [typeof(RandomHSVModel)] = model => mapper.Map(model, x.GetRequiredService<RandomHSV>()),
                    [typeof(TransformSRTModel)] = model => mapper.Map(model, x.GetRequiredService<TransformSRT>()),
                    [typeof(RandomXYZModel)] = model => mapper.Map(model, x.GetRequiredService<RandomXYZ>()),
                    [typeof(GaussianXYZModel)] = model => mapper.Map(model, x.GetRequiredService<GaussianXYZ>()),
                    [typeof(ScaleModel)] = model => mapper.Map(model, x.GetRequiredService<Scale>()),
                    [typeof(RotationModel)] = model => mapper.Map(model, x.GetRequiredService<Rotation>()),
                    [typeof(TranslateModel)] = model => mapper.Map(model, x.GetRequiredService<Translate>()),
                    [typeof(LinearXYZModel)] = model => mapper.Map(model, x.GetRequiredService<LinearXYZ>()),
                    [typeof(LFOModel)] = model => mapper.Map(model, x.GetRequiredService<LFO>()),
                    [typeof(RandomScaleModel)] = model => mapper.Map(model, x.GetRequiredService<RandomScale>()),
                    [typeof(StepperModel)] = model => mapper.Map(model, x.GetRequiredService<Stepper>()),

                    [typeof(CameraLFOModel)] = model => mapper.Map(model, x.GetRequiredService<CameraLFO>()),
                    [typeof(CameraRandomModel)] = model => mapper.Map(model, x.GetRequiredService<CameraRandom>()),

                    [typeof(RandomPositionModel)] = model => mapper.Map(model, x.GetRequiredService<RandomPosition>()),
                    [typeof(RandomRotationModel)] = model => mapper.Map(model, x.GetRequiredService<RandomRotation>()),
                };


                return new ControlFactory(finalDictionary, controlModelFactory, x.GetRequiredService<ControlRepository>());
            });


        }
    }
}
