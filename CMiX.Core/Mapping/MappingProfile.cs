// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
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
            Profiles.Add(new ModifiersMappingProfile());
            Profiles.Add(new PrefabMappingProfile());
            Profiles.Add(new MeshMappingProfile());
            Profiles.Add(new RenderingMappingProfile());
            Profiles.Add(new BeatMappingProfile());
            Profiles.Add(new LightMappingProfile());
            CreateMap<GenericValue<FontStyle>, GenericValueModel<FontStyle>>().ReverseMap();
        }

        public List<Profile> Profiles { get; set; }
    }
}
