// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
{
    public class TransformMappingProfile : Profile
    {
        public TransformMappingProfile()
        {
            CreateMap<GenericValue<TransformType>, GenericValueModel<TransformType>>().ReverseMap();
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();
            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
        }
    }
}
