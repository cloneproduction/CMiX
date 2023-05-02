// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Mapping
{
    public class MeshMappingProfile : Profile
    {
        public MeshMappingProfile()
        {
            CreateMap<Mesh, MeshModel>().ReverseMap();
            CreateMap<GenericValue<MeshType>, GenericValueModel<MeshType>>().ReverseMap();
        }
    }
}
