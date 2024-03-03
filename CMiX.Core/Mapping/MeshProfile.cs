// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Mapping
{
    public class MeshProfile : Profile
    {
        public MeshProfile()
        {
            CreateMap<Mesh, MeshModel>().ReverseMap();
        }
    }
}
