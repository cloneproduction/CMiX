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
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class TextEntity : ObservableObject, IControl, IPrefab, IModifiable, IDisposable
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

        public Guid ID { get; set; } = Guid.NewGuid();
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

        public IControlModel ToModel() => new TextEntityModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TransformSRT = (TransformSRTModel)TransformSRT.ToModel(),
            Text = (GenericValueModel<string>)Text.ToModel(),
            Size = (GenericValueModel<float>)Size.ToModel(),
            Color = (GenericValueModel<string>)Color.ToModel(),
            Style = (GenericValueModel<FontStyle>)Style.ToModel(),
            FontFamily = (GenericValueModel<string>)FontFamily.ToModel(),
            LineHeight = (GenericValueModel<float>)LineHeight.ToModel(),
            Width = (GenericValueModel<float>)Width.ToModel(),
            HorizontalAlignment = (GenericValueModel<HorizontalAlignment>)HorizontalAlignment.ToModel(),
            Anchor = (GenericValueModel<Anchor>)Anchor.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TextEntityModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            TransformSRT.FromModel(m.TransformSRT);
            Text.FromModel(m.Text);
            Size.FromModel(m.Size);
            Color.FromModel(m.Color);
            Style.FromModel(m.Style);
            FontFamily.FromModel(m.FontFamily);
            LineHeight.FromModel(m.LineHeight);
            Width.FromModel(m.Width);
            HorizontalAlignment.FromModel(m.HorizontalAlignment);
            Anchor.FromModel(m.Anchor);

            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose()
        {
            ModifierManager.ClearAll();
        }
    }
}
