// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Camera;
using CMiX.Core.Presentations.Modifiers.Color;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Modifiers;

namespace CMiX.Core.Mapper
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
