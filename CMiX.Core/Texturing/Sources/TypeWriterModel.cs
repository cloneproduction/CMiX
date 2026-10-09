// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record TypeWriterModel : IControlModel, IPrefabModel, ITextureSourceModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<string> StringControl { get; set; } = new("CMiX");
        public GenericValueModel<string> FontColor { get; set; } = new("#ff000000");
        public GenericValueModel<string> BackgroundColor { get; set; } = new("#00000000");
        public Integer2Model Resolution { get; set; } = new(1024, 1024);
        public GenericValueModel<bool> UseCompositionResolution { get; set; } = new(false);
        public Vector2Model Position { get; set; } = new();
        public GenericValueModel<float> FontSize { get; set; } = new(0.45f);
        public GenericValueModel<string> FontFamily { get; set; } = new("Arial");
        public GenericValueModel<FontStyle> Style { get; set; } = new(FontStyle.Normal);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
    }
}
