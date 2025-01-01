// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Text;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Compositing
{
    public class TextEntityModel : IControlModel, IPrefabModel
    {
        public TextEntityModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            ModifierManager = new PrefabManagerModel();
            ColorPaletteManager = new PrefabManagerModel();
            Text = new GenericValueModel<string>("CMiX");
            Size = new GenericValueModel<float>(0.8f);
            Color = new GenericValueModel<string>("#ffffffff");
            Style = new GenericValueModel<FontStyle>(FontStyle.Normal);
            FontFamily = new GenericValueModel<string>("Arial");
            LineHeight = new GenericValueModel<float>(1.50f);
            Width = new GenericValueModel<float>(9.0f);
            HorizontalAlignment = new GenericValueModel<HorizontalAlignment>(Core.Text.HorizontalAlignment.Left);
            Anchor = new GenericValueModel<Anchor>(Core.Text.Anchor.Center);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public PrefabManagerModel ColorPaletteManager { get; set; }
        public GenericValueModel<string> Text { get; set; }
        public GenericValueModel<float> Size { get; set; }
        public GenericValueModel<string> Color { get; set; }
        public GenericValueModel<FontStyle> Style { get; set; }
        public GenericValueModel<string> FontFamily { get; set; }
        public GenericValueModel<float> LineHeight { get; set; }
        public GenericValueModel<float> Width { get; set; }
        public GenericValueModel<HorizontalAlignment> HorizontalAlignment { get; set; }
        public GenericValueModel<Anchor> Anchor { get; set; }
    }
}
