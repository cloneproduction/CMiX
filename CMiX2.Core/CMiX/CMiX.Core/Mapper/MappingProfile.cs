// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Mesh, MeshModel>().ReverseMap();
            CreateMap<ModifierManager, ModifierManagerModel>().ReverseMap();
            CreateMap<Texture, TextureModel>().ReverseMap();
            CreateMap<VideoIn, VideoInModel>().ReverseMap();
            CreateMap<VideoPlayer, VideoPlayerModel>().ReverseMap();

            CreateMap<Button, ButtonModel>().ReverseMap();

            CreateMap<SamplerState, SamplerStateModel>().ReverseMap();

            CreateMap<GenericValue<FontStyle>, GenericValueModel<FontStyle>>().ReverseMap();
            CreateMap<TypeWriter, TypeWriterModel>().ReverseMap();


            CreateMap<ProceduralSelector, ProceduralSelectorModel>().ReverseMap();
            CreateMap<GenericValue<TextureSourceName>, GenericValueModel<TextureSourceName>>().ReverseMap();
            CreateMap<Gradient, GradientModel>().ReverseMap();
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap();

            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap();
            //CreateMap<PrefabManager, PrefabManagerModel>().ReverseMap();

            CreateMap<Transform2D, Transform2DModel>().ReverseMap();
            CreateMap<ColorSelector, ColorSelectorModel>().ReverseMap();


            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();

            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();
            CreateMap<IntegerValue, IntegerValueModel>().ReverseMap();
            CreateMap<FloatValue, FloatValueModel>().ReverseMap();
            CreateMap<BooleanValue, BooleanValueModel>().ReverseMap();
            CreateMap<StringValue, StringValueModel>().ReverseMap();
        }
    }
}
