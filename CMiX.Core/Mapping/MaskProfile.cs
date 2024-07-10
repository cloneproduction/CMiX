// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing;

namespace CMiX.Core.Mapping
{
    public class MaskProfile : Profile
    {
        public MaskProfile()
        {
            CreateMap<MaskTexture, MaskTextureModel>().ReverseMap().ConstructUsingServiceLocator(); ;
        }
    }
}
