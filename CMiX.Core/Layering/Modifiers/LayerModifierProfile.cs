// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Layering.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class LayerModifierProfile : Profile
    {
        public LayerModifierProfile()
        {
            CreateMap<SelectRandomEntity, SelectRandomEntityModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<SelectRandomEntity, SelectRandomEntityModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
