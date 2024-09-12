// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Mapping
{
    public class EntityModifierProfile : Profile
    {
        public EntityModifierProfile()
        {
            CreateMap<TransformSRT, TransformSRTModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Scale, ScaleModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Rotation, RotationModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Translate, TranslateModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomXYZ, RandomXYZModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomTexCoord, RandomTexCoordModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<LinearXYZ, LinearXYZModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<CircularSpread, CircularSpreadModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<LFO, LFOModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomRotation, RandomRotationModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomScale, RandomScaleModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<RandomPosition, RandomPositionModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Grid, GridModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Billboard, BillboardModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<TransformSRT, TransformSRTModel>()
                .Include<Rotation, RotationModel>()
                .Include<Scale, ScaleModel>()
                .Include<Translate, TranslateModel>()
                .Include<RandomXYZ, RandomXYZModel>()
                .Include<RandomTexCoord, RandomTexCoordModel>()
                .Include<LinearXYZ, LinearXYZModel>()
                .Include<CircularSpread, CircularSpreadModel>()
                .Include<LFO, LFOModel>()
                .Include<RandomRotation, RandomRotationModel>()
                .Include<RandomScale, RandomScaleModel>()
                .Include<RandomHSV, RandomHSVModel>()
                .Include<RandomPosition, RandomPositionModel>()
                .Include<Grid, GridModel>()
                .Include<Billboard, BillboardModel>()
                .ReverseMap().ConstructUsingServiceLocator(); ;
        }
    }
}
