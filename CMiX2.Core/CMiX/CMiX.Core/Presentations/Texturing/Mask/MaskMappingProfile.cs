// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Texturing
{
    public class MaskMappingProfile : Profile
    {
        public MaskMappingProfile()
        {
            CreateMap<Mask, MaskModel>().ReverseMap();
            CreateMap<GenericValue<MaskChannel>, GenericValueModel<MaskChannel>>().ReverseMap();
            CreateMap<GenericValue<MaskMode>, GenericValueModel<MaskMode>>().ReverseMap();
        }
    }
}
