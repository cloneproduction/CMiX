// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Camera;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Components.Composition;
using CMiX.Core.Presentations.Components.Entity;
using CMiX.Core.Presentations.Components.Layer;
using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Material;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources.BubbleNoise;
using CMiX.Core.Texturing.Sources.Gradient;
using CMiX.Core.Texturing.Sources.TypeWriter;
using CMiX.Core.Texturing.Sources.VideoIn;
using CMiX.Core.Texturing.Sources.VideoPlayer;

namespace CMiX.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Mesh, MeshModel>().ReverseMap();

            CreateMap<ModifierManager, ModifierManagerModel>().ReverseMap();
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

            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
            CreateMap<GenericValue<ModifierMode>, GenericValueModel<ModifierMode>>().ReverseMap();

            CreateMap<IModifier, TransformSRTModel>().ReverseMap();

            CreateMap<ProceduralSelector, ProceduralSelectorModel>().ReverseMap();
            CreateMap<GenericValue<TextureSourceName>, GenericValueModel<TextureSourceName>>().ReverseMap();
            CreateMap<Gradient, GradientModel>().ReverseMap();
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap();

            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap();

            CreateMap<Transform2D, Transform2DModel>().ReverseMap();
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();

            CreateMap<ColorSelector, ColorSelectorModel>().ReverseMap();
            CreateMap<OutputSettings, OutputSettingsModel>().ReverseMap();

            CreateMap<PrefabManager<Layer>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Entity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Camera>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<LightEntity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Material>, PrefabManagerModel>().ReverseMap();

            CreateMap<PrefabContainer, PrefabContainerModel>().ReverseMap();

            CreateMap<IPrefab, IPrefabModel>()
                .Include<Composition, CompositionModel>()
                .Include<Layer, LayerModel>()
                .Include<Entity, EntityModel>()
                .Include<Camera, CameraModel>()
                .Include<LightEntity, LightEntityModel>()
                .Include<Material, MaterialModel>()
                .ReverseMap();

            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Layer, LayerModel>().ReverseMap();
            CreateMap<Composition, CompositionModel>().ReverseMap();
            CreateMap<Camera, CameraModel>().ReverseMap();
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();
            CreateMap<Material, MaterialModel>().ReverseMap();

            CreateMap<AmbientOcclusion, AmbientOcclusionModel>().ReverseMap();
            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap();

            CreateMap<Vector3, Vector3Model>().ReverseMap();
            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();
            CreateMap<IntegerValue, IntegerValueModel>().ReverseMap();
            CreateMap<FloatValue, FloatValueModel>().ReverseMap();
            CreateMap<BooleanValue, BooleanValueModel>().ReverseMap();
            CreateMap<StringValue, StringValueModel>().ReverseMap();
        }
    }
}
