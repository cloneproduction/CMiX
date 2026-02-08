// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Text;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class TextEntity : ObservableObject, IControl, IPrefab, IModifiable
    {
        public TextEntity(PrefabService prefabService,
                          PrefabManager prefabManager,
                          TransformSRT transformSRT,
                          GenericValue<string> text,
                          GenericValue<float> size,
                          GenericValue<string> color,
                          GenericValue<FontStyle> style,
                          GenericValue<string> fontFamily,
                          GenericValue<float> lineHeight,
                          GenericValue<float> width,
                          GenericValue<HorizontalAlignment> horizontalAlignment,
                          GenericValue<Anchor> anchor)
        {
            PrefabService = prefabService;
            ModifierManager = prefabManager;
            TransformSRT = transformSRT;
            Text = text;
            Size = size;
            Color = color;
            Style = style;
            FontFamily = fontFamily;
            LineHeight = lineHeight;
            Width = width;
            HorizontalAlignment = horizontalAlignment;
            Anchor = anchor;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public GenericValue<string> Text { get; set; }
        public GenericValue<float> Size { get; set; }
        public GenericValue<string> Color { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public GenericValue<string> FontFamily { get; set; }
        public GenericValue<float> LineHeight { get; set; }
        public GenericValue<float> Width { get; set; }
        public GenericValue<HorizontalAlignment> HorizontalAlignment { get; set; }
        public GenericValue<Anchor> Anchor { get; set; }

        [ObservableProperty]
        private bool transformSRTIsExpanded = true;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = true;

        [ObservableProperty]
        private bool settingsIsExpanded = true;
    }
}
