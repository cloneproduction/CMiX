//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using AutoMapper;
//using CMiX.Core.Animations;
//using CMiX.Core.Assets.Mesh;
//using CMiX.Core.BaseControls;
//using CMiX.Core.Colors;
//using CMiX.Core.Compositing;
//using CMiX.Core.Materials;
//using CMiX.Core.Materials.Modifiers;
//using CMiX.Core.Modifiers;
//using CMiX.Core.Networking.Messenger;
//using CMiX.Core.Networking.Servers;
//using CMiX.Core.Prefabs;
//using CMiX.Core.Prefabs.Managers;
//using CMiX.Core.Rendering;
//using CMiX.Core.Rendering.Cameras;
//using CMiX.Core.Rendering.Cameras.Modifiers;
//using CMiX.Core.Rendering.Lights;
//using CMiX.Core.Text.Modifiers;
//using CMiX.Core.Texturing;
//using CMiX.Core.Texturing.Filters;
//using CMiX.Core.Transformation;
//using CMiX.Core.ViewModels;

//namespace CMiX.Core.Mapping
//{
//    public class ControlsProfile : Profile
//    {
//        public List<Type> OtherTypes { get; } = new();
//        public List<Type> RegisteredTypes { get; } = new();

//        public ControlsProfile()
//        {
//            this.CreateMap<Integer2, Integer2Model>().ReverseMap().ConstructUsingServiceLocator(); // map Integer2 <-> model
//            this.CreateMap<GenericValue<string>, GenericValueModel<string>>().ReverseMap().ConstructUsingServiceLocator(); // open generic works too
//            this.CreateMap<GenericValue<bool>, GenericValueModel<bool>>().ReverseMap().ConstructUsingServiceLocator();
//            this.CreateMap<GenericValue<Int32>, GenericValueModel<Int32>>().ReverseMap().ConstructUsingServiceLocator();
//            this.CreateMap<GenericValue<float>, GenericValueModel<float>>().ReverseMap().ConstructUsingServiceLocator();

//            this.MapControlByInterface(RegisteredTypes, typeof(ITextureSource));
//            this.MapControlByInterface(RegisteredTypes, typeof(ITextureFilter));
//            this.MapControlByInterface(RegisteredTypes, typeof(IModifier));


//            this.MapControlByConvention(
//                typeof(Easing));

//            this.MapControlByConvention(
//                typeof(Transform2D));

//            this.MapControlByConvention(
//                typeof(DiffuseTexture),
//                typeof(MaskTexture),
//                typeof(SamplerState));

//            this.MapControlByConvention(
//                typeof(MaskTexture));

//            this.MapControlByConvention(
//                typeof(Split),
//                typeof(CharWriter));

//            this.MapControlByConvention(
//                typeof(Server),
//                typeof(ServerManager));

//            this.MapControlByConvention(
//                typeof(OutputSettings),
//                typeof(AmbientOcclusion),
//                typeof(LocalReflection));

//            this.MapControlByConvention(
//                typeof(PrefabService),
//                typeof(PrefabManager),
//                typeof(ManagerData),
//                typeof(EmptyPrefab),
//                typeof(Composition),
//                typeof(Layer),
//                typeof(LayerSettings),
//                typeof(Entity),
//                typeof(Camera),
//                typeof(TextEntity),
//                typeof(Color));

//            this.MapControlByConvention(
//                typeof(ModifierModeSelector));

//            this.MapControlByConvention(
//                typeof(Mesh),
//                typeof(Text3DSettings),
//                typeof(MeshModel));

//            this.MapControlByConvention(
//                typeof(Material),
//                typeof(MaterialSettings),
//                typeof(SelectRandomTexture));

//            this.MapControlByConvention(
//                typeof(MasterBeat),
//                typeof(BeatModifier));

//            this.MapControlByConvention(
//                typeof(LightEntity),
//                typeof(LightSettings));

//            this.MapControlByConvention(
//                typeof(Coloration));

//            this.MapControlByConvention(
//                typeof(CameraSettings),
//                typeof(CameraLFO),
//                typeof(CameraRandom));

//            this.MapControlByConvention(
//                typeof(DirectionXYZ),
//                typeof(DirectionXY),
//                typeof(AssetSelector),
//                typeof(Button)
//                );
//        }
//    }
//}
