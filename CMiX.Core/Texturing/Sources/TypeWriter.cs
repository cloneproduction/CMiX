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
    public partial class TypeWriter : ObservableObject, ITextureSource, IDisposable
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
                          Vector2 position)
        {
            TextureModifierManager = textureModifierManager;
            PrefabService = prefabService;
            StringControl = stringControl;
            FontFamily = fontFamily;
            FontSize = fontSize;
            Style = fontStyle;
            FontColor = fontColor;
            BackgroundColor = backgroundColor;
            Resolution = resolution;
            Position = position;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<string> StringControl { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public GenericValue<string> FontFamily { get; set; }
        public GenericValue<float> FontSize { get; set; }
        public GenericValue<string> FontColor { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public Integer2 Resolution { get; set; }
        public Vector2 Position { get; set; }

        public PrefabManager TextureModifierManager { get; set; }
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TypeWriterModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            StringControl = (GenericValueModel<string>)StringControl.ToModel(),
            FontColor = (GenericValueModel<string>)FontColor.ToModel(),
            BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            Position = (Vector2Model)Position.ToModel(),
            FontSize = (GenericValueModel<float>)FontSize.ToModel(),
            FontFamily = (GenericValueModel<string>)FontFamily.ToModel(),
            Style = (GenericValueModel<FontStyle>)Style.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TypeWriterModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            StringControl.FromModel(m.StringControl);
            FontColor.FromModel(m.FontColor);
            BackgroundColor.FromModel(m.BackgroundColor);
            Resolution.FromModel(m.Resolution);
            Position.FromModel(m.Position);
            FontSize.FromModel(m.FontSize);
            FontFamily.FromModel(m.FontFamily);
            Style.FromModel(m.Style);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
        }

        // The filter modifiers are reachable through this manager alone, so a texture torn down
        // without disposing it leaves their repository and their deleter registrations behind.
        public void Dispose()
        {
            TextureModifierManager.Dispose();
        }
    }
}
