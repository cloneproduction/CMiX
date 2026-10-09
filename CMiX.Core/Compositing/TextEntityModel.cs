// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Text;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Transformation;

namespace CMiX.Core.Compositing
{
    public record TextEntityModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabManagerModel ColorPaletteManager { get; set; } = new();
        public TransformSRTModel TransformSRT { get; set; } = new();
        public GenericValueModel<string> Text { get; set; } = new("CMiX");
        public GenericValueModel<float> Size { get; set; } = new(0.8f);
        public GenericValueModel<string> Color { get; set; } = new("#ffffffff");
        public GenericValueModel<FontStyle> Style { get; set; } = new(FontStyle.Normal);
        public GenericValueModel<string> FontFamily { get; set; } = new("Arial");
        public GenericValueModel<float> LineHeight { get; set; } = new(1.50f);
        public GenericValueModel<float> Width { get; set; } = new(9.0f);
        public GenericValueModel<HorizontalAlignment> HorizontalAlignment { get; set; } = new(Core.Text.HorizontalAlignment.Left);
        public GenericValueModel<Anchor> Anchor { get; set; } = new(Core.Text.Anchor.Center);
    }
}
