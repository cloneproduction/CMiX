// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Beat;
using CMiX.Core.Presentations;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Color;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Modifiers;

namespace CMiX.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Profiles = new List<Profile>();
            Profiles.Add(new FiltersMappingProfile());
            Profiles.Add(new MaskMappingProfile());
            Profiles.Add(new BaseControlMappingProfile());
            Profiles.Add(new CameraMappingProfile());
            Profiles.Add(new SourceMappingProfile());
            Profiles.Add(new TransformMappingProfile());
            Profiles.Add(new MaterialMappingProfile());
            Profiles.Add(new AnimationMappingProfile());
            Profiles.Add(new TexturingMappingProfile());

            CreateMap<Mesh, MeshModel>().ReverseMap();
            CreateMap<GenericValue<MeshType>, GenericValueModel<MeshType>>().ReverseMap();

            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap();
            CreateMap<BeatModifier, BeatModifierModel>().ReverseMap();

            CreateMap<ModifierManager, ModifierManagerModel>().ReverseMap();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap();

            CreateMap<Button, ButtonModel>().ReverseMap();

            CreateMap<SamplerState, SamplerStateModel>().ReverseMap();
            CreateMap<GenericValue<TextureAddressMode>, GenericValueModel<TextureAddressMode>>().ReverseMap();

            CreateMap<GenericValue<FontStyle>, GenericValueModel<FontStyle>>().ReverseMap();


            CreateMap<GenericValue<ModifierMode>, GenericValueModel<ModifierMode>>().ReverseMap();

            CreateMap<Transform2D, Transform2DModel>().ReverseMap();

            CreateMap<OutputSettings, OutputSettingsModel>().ReverseMap();

            CreateMap<PrefabManager<Material>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Layer>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Entity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<LightEntity>, PrefabManagerModel>().ReverseMap();


            CreateMap<PrefabContainer, PrefabContainerModel>().ReverseMap();

            CreateMap<IModifier, IModifierModel>().ReverseMap();

            CreateMap<IPrefab, IPrefabModel>()
                .Include<Composition, CompositionModel>()
                .Include<Layer, LayerModel>()
                .Include<Entity, EntityModel>()
                .Include<Camera, CameraModel>()
                .Include<LightEntity, LightEntityModel>()
                .Include<Material, MaterialModel>()
                .Include<MasterBeat, MasterBeatModel>()
                .ReverseMap();

            //CreateMap<Component, ComponentModel>().ReverseMap();
            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Layer, LayerModel>().ReverseMap();
            CreateMap<Composition, CompositionModel>().ReverseMap();
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();

            CreateMap<AmbientOcclusion, AmbientOcclusionModel>().ReverseMap();
        }

        public List<Profile> Profiles { get; set; }
    }
}
