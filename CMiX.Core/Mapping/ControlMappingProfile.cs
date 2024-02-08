// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Mapping
{
    public class ControlMappingProfile: Profile
    {
        public ControlMappingProfile(Type controlType, Type controlModelType)
        {
            CreateMap(controlType, controlModelType).ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include(controlType, controlModelType)
                .ReverseMap();
        }
    }
}
