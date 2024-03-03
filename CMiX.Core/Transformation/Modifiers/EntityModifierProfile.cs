// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Colors.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class EntityModifierProfile : Profile
    {
        public EntityModifierProfile()
        {
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap();
            CreateMap<Scale, ScaleModel>().ReverseMap();
            CreateMap<Rotation, RotationModel>().ReverseMap();
            CreateMap<Translate, TranslateModel>().ReverseMap();
            CreateMap<RandomXYZ, RandomXYZModel>().ReverseMap();
            CreateMap<GaussianXYZ, GaussianXYZModel>().ReverseMap();
            CreateMap<LinearXYZ, LinearXYZModel>().ReverseMap();
            CreateMap<LFO, LFOModel>().ReverseMap();
            CreateMap<RandomRotation, RandomRotationModel>().ReverseMap();
            CreateMap<RandomScale, RandomScaleModel>().ReverseMap();
            CreateMap<Stepper, StepperModel>().ReverseMap();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap();
            CreateMap<RandomPosition, RandomPositionModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<TransformSRT, TransformSRTModel>()
                .Include<Rotation, RotationModel>()
                .Include<Scale, ScaleModel>()
                .Include<Translate, TranslateModel>()
                .Include<RandomXYZ, RandomXYZModel>()
                .Include<GaussianXYZ, GaussianXYZModel>()
                .Include<LinearXYZ, LinearXYZModel>()
                .Include<LFO, LFOModel>()
                .Include<RandomRotation, RandomRotationModel>()
                .Include<RandomScale, RandomScaleModel>()
                .Include<Stepper, StepperModel>()
                .Include<RandomHSV, RandomHSVModel>()
                .Include<RandomPosition, RandomPositionModel>()
                .ReverseMap();
        }
    }
}
