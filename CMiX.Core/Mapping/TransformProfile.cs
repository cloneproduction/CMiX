// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Transformation;

namespace CMiX.Core.Mapping
{
    public class TransformProfile : Profile
    {
        public TransformProfile()
        {
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
            CreateMap<Transform2D, Transform2DModel>().ReverseMap();
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();
        }
    }
}
