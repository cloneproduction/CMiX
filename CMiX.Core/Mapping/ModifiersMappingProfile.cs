// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Mapping
{
    public class ModifiersMappingProfile : Profile
    {
        public ModifiersMappingProfile()
        {
            CreateMap<ModifierManager, ModifierManagerModel>().ReverseMap();
            CreateMap<IModifier, IModifierModel>().ReverseMap();
            CreateMap<GenericValue<ModifierMode>, GenericValueModel<ModifierMode>>().ReverseMap();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap();
            CreateMap<RandomXYZ, RandomXYZModel>().ReverseMap();
            CreateMap<RandomScale, RandomScaleModel>().ReverseMap();
            CreateMap<LinearXYZ, LinearXYZModel>().ReverseMap();
            CreateMap<LFO, LFOModel>().ReverseMap();
            CreateMap<Stepper, StepperModel>().ReverseMap();

            CreateMap<CameraLFO, CameraLFOModel>().ReverseMap();
            CreateMap<CameraRandom, CameraRandomModel>().ReverseMap();
        }
    }
}
