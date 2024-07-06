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
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<Scale, ScaleModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<Rotation, RotationModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<Translate, TranslateModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<RandomXYZ, RandomXYZModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<LinearXYZ, LinearXYZModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<CircularSpread, CircularSpreadModel>().ReverseMap().ConstructUsingServiceLocator(); ;

            CreateMap<LFO, LFOModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<RandomRotation, RandomRotationModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<RandomScale, RandomScaleModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<Stepper, StepperModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<RandomPosition, RandomPositionModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<Grid, GridModel>().ReverseMap().ConstructUsingServiceLocator(); ;

            CreateMap<IControl, IControlModel>()
                .Include<TransformSRT, TransformSRTModel>()
                .Include<Rotation, RotationModel>()
                .Include<Scale, ScaleModel>()
                .Include<Translate, TranslateModel>()
                .Include<RandomXYZ, RandomXYZModel>()
                .Include<LinearXYZ, LinearXYZModel>()
                .Include<CircularSpread, CircularSpreadModel>()
                .Include<LFO, LFOModel>()
                .Include<RandomRotation, RandomRotationModel>()
                .Include<RandomScale, RandomScaleModel>()
                .Include<Stepper, StepperModel>()
                .Include<RandomHSV, RandomHSVModel>()
                .Include<RandomPosition, RandomPositionModel>()
                .Include<Grid, GridModel>()
                .ReverseMap().ConstructUsingServiceLocator(); ;
        }
    }
}
