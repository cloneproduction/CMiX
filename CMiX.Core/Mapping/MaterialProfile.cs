// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Materials;
using CMiX.Core.Materials.Modifiers;

namespace CMiX.Core.Mapping
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<MaterialSettings, MaterialSettingsModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<SelectRandomTexture, SelectRandomTextureModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<Material, MaterialModel>()
                .Include<MaterialSettings, MaterialSettingsModel>()
                .Include<SelectRandomTexture, SelectRandomTextureModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
