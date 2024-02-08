// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Mapping
{
    public class FiltersMappingProfile : Profile
    {
        public FiltersMappingProfile()
        {

            var textureModifierPairs = new Dictionary<Type, Type>();

            foreach (var textureModifierPair in textureModifierPairs)
            {
                CreateMap<IControl, IControlModel>()
                    .Include(textureModifierPair.Key, textureModifierPair.Value)
                    .ReverseMap();

                CreateMap(textureModifierPair.Key, textureModifierPair.Value).ReverseMap();
            }

            CreateMap<GenericValue<InvertChannel>, GenericValueModel<InvertChannel>>().ReverseMap();
        }
    }
}
