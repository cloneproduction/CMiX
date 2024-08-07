// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Text.Modifiers;

namespace CMiX.Core.Mapping
{
    public class TextProfile : Profile
    {
        public TextProfile()
        {
            CreateMap<Split, SplitModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<CharWriter, CharWriterModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(Split), typeof(SplitModel))
                .Include(typeof(CharWriter), typeof(CharWriterModel))
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
