// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Mapping
{
    public class MaskMappingProfile : Profile
    {
        public MaskMappingProfile()
        {
            CreateMap<MaskTexture, MaskModel>().ReverseMap();
            CreateMap<GenericValue<MaskChannel>, GenericValueModel<MaskChannel>>().ReverseMap();
            CreateMap<GenericValue<MaskMode>, GenericValueModel<MaskMode>>().ReverseMap();
        }
    }
}
