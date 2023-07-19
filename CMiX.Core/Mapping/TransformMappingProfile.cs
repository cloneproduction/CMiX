// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Transformation;

namespace CMiX.Core.Mapping
{
    public class TransformMappingProfile : Profile
    {
        public TransformMappingProfile()
        {
            CreateMap<Transform2D, Transform2DModel>().ReverseMap();
            CreateMap<GenericValue<TransformType>, GenericValueModel<TransformType>>().ReverseMap();
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();
            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
        }
    }
}
