// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Mapping
{
    public class ControlsProfile : Profile
    {
        public List<Type> OtherTypes { get; } = new();
        public List<Type> RegisteredTypes { get; } = new();

        public ControlsProfile()
        {
            this.MapGenericValueByConvention();

            this.MapControlByInterface(OtherTypes, typeof(ITextureSource));
            this.MapControlByInterface(RegisteredTypes, typeof(ITextureFilter));
            this.MapControlByInterface(OtherTypes, typeof(IModifier));

            this.MapAllControlsByConvention();
        }
    }
}
