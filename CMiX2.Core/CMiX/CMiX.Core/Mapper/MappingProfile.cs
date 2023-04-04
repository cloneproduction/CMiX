// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Color;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.ViewModels.Components;
using CMiX.Core.Presentations.ViewModels.Modifiers;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ColorSelector, ColorSelectorModel>()
                .ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => src.SelectedColor.ToString()))
                .ReverseMap().ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => (Color)ColorConverter.ConvertFromString(src.SelectedColor)));


            CreateMap<Mesh, MeshModel>().ReverseMap();
            CreateMap<GenericValue<MeshType>, GenericValueModel<MeshType>>().ReverseMap();

            CreateMap<BeatModifier, BeatModifierModel>().ReverseMap();

            CreateMap<Easing, EasingModel>().ReverseMap();
            CreateMap<GenericValue<EasingMode>, GenericValueModel<EasingMode>>().ReverseMap();
            CreateMap<GenericValue<EasingFunction>, GenericValueModel<EasingFunction>>().ReverseMap();


            CreateMap<ModifierManager, ModifierManagerModel>().ReverseMap();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap();
            CreateMap<Blur, BlurModel>().ReverseMap();


            CreateMap<Texture, TextureModel>().ReverseMap();
            CreateMap<VideoIn, VideoInModel>().ReverseMap();
            CreateMap<VideoPlayer, VideoPlayerModel>().ReverseMap();

            CreateMap<Button, ButtonModel>().ReverseMap();

            CreateMap<SamplerState, SamplerStateModel>().ReverseMap();

            CreateMap<GenericValue<TextureAddressMode>, GenericValueModel<TextureAddressMode>>().ReverseMap();
            CreateMap<GenericValue<FontStyle>, GenericValueModel<FontStyle>>().ReverseMap();
            CreateMap<GenericValue<BlendModeEnum>, GenericValueModel<BlendModeEnum>>().ReverseMap();

            CreateMap<TypeWriter, TypeWriterModel>().ReverseMap();

            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();
            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
            CreateMap<GenericValue<ModifierMode>, GenericValueModel<ModifierMode>>().ReverseMap();

            

            CreateMap<ProceduralSelector, ProceduralSelectorModel>().ReverseMap();
            CreateMap<GenericValue<TextureSourceName>, GenericValueModel<TextureSourceName>>().ReverseMap();
            CreateMap<Gradient, GradientModel>().ReverseMap();
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap();

            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap();

            CreateMap<Transform2D, Transform2DModel>().ReverseMap();

            CreateMap<OutputSettings, OutputSettingsModel>().ReverseMap();

            CreateMap<PrefabManager<Layer>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Entity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Camera>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<LightEntity>, PrefabManagerModel>().ReverseMap();

            CreateMap<PrefabManager<Material>, PrefabManagerModel>().ReverseMap();
            CreateMap<GenericValue<PipelineType>, GenericValueModel<PipelineType>>().ReverseMap();
            CreateMap<GenericValue<TransparencyType>, GenericValueModel<TransparencyType>>().ReverseMap();
            CreateMap<GenericValue<CullModeType>, GenericValueModel<CullModeType>>().ReverseMap();

            CreateMap<Mask, MaskModel>().ReverseMap();
            CreateMap<GenericValue<MaskChannel>, GenericValueModel<MaskChannel>>().ReverseMap();

            CreateMap<PrefabContainer, PrefabContainerModel>().ReverseMap();

            CreateMap<IModifier, IModifierModel>().ReverseMap();

            CreateMap<Material, MaterialModel>().ReverseMap();

            CreateMap<IPrefab, IPrefabModel>()
                .Include<Composition, CompositionModel>()
                .Include<Layer, LayerModel>()
                .Include<Entity, EntityModel>()
                .Include<Camera, CameraModel>()
                .Include<LightEntity, LightEntityModel>()
                .Include<Material, MaterialModel>()
                .ReverseMap();

            CreateMap<Component, ComponentModel>().ReverseMap();
            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Layer, LayerModel>().ReverseMap();
            CreateMap<Composition, CompositionModel>().ReverseMap();
            CreateMap<Camera, CameraModel>().ReverseMap();
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();
            CreateMap<Material, MaterialModel>().ReverseMap();


            CreateMap<AmbientOcclusion, AmbientOcclusionModel>().ReverseMap();
            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap();

            CreateMap<Vector3, Vector3Model>().ConstructUsing(src => new Vector3Model()).ReverseMap();
            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();
            CreateMap<IntegerValue, IntegerValueModel>().ReverseMap();
            CreateMap<FloatValue, FloatValueModel>().ReverseMap();
            CreateMap<BooleanValue, BooleanValueModel>().ReverseMap();
            CreateMap<StringValue, StringValueModel>().ReverseMap();
        }
    }
}
