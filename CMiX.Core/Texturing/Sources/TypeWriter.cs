// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class TypeWriter : TextureSourceBase, ITextureSource
    {
        public TypeWriter(PrefabManager textureModifierManager,
                          PrefabService prefabService,
                          GenericValue<string> stringControl,
                          GenericValue<string> fontFamily,
                          GenericValue<float> fontSize,
                          GenericValue<FontStyle> fontStyle,
                          GenericValue<string> fontColor,
                          GenericValue<string> backgroundColor,
                          Integer2 resolution,
                          GenericValue<bool> useCompositionResolution,
                          Vector2 position)
            : base(prefabService, textureModifierManager)
        {
            StringControl = stringControl;
            FontFamily = fontFamily;
            FontSize = fontSize;
            Style = fontStyle;
            FontColor = fontColor;
            BackgroundColor = backgroundColor;
            Resolution = resolution;
            UseCompositionResolution = useCompositionResolution;
            Position = position;
        }

        public GenericValue<string> StringControl { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public GenericValue<string> FontFamily { get; set; }
        public GenericValue<float> FontSize { get; set; }
        public GenericValue<string> FontColor { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public Integer2 Resolution { get; set; }
        public GenericValue<bool> UseCompositionResolution { get; set; }
        public Vector2 Position { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TypeWriterModel
            {
                StringControl = (GenericValueModel<string>)StringControl.ToModel(),
                FontColor = (GenericValueModel<string>)FontColor.ToModel(),
                BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel(),
                Resolution = (Integer2Model)Resolution.ToModel(),
                UseCompositionResolution = (GenericValueModel<bool>)UseCompositionResolution.ToModel(),
                Position = (Vector2Model)Position.ToModel(),
                FontSize = (GenericValueModel<float>)FontSize.ToModel(),
                FontFamily = (GenericValueModel<string>)FontFamily.ToModel(),
                Style = (GenericValueModel<FontStyle>)Style.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TypeWriterModel)model;
            LoadBaseModel(m);
            StringControl.FromModel(m.StringControl);
            FontColor.FromModel(m.FontColor);
            BackgroundColor.FromModel(m.BackgroundColor);
            Resolution.FromModel(m.Resolution);
            UseCompositionResolution.FromModel(m.UseCompositionResolution);
            Position.FromModel(m.Position);
            FontSize.FromModel(m.FontSize);
            FontFamily.FromModel(m.FontFamily);
            Style.FromModel(m.Style);
        }
    }
}
