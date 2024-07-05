// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Modifiers
{
    public class ModifierProfile : Profile
    {
        public ModifierProfile()
        {
            CreateMap<ModifierModeSelector, ModifierModeSelectorModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<ModifierMode3DSelector, ModifierMode3DSelectorModel>().ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
