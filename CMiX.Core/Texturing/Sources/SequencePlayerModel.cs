// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record SequencePlayerModel : IPrefabModel, ITextureSourceModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ButtonModel DoSeek { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<int> SeekFrame { get; set; } = new(0);
        public GenericValueModel<bool> Play { get; set; } = new(true);
        public GenericValueModel<float> FPS { get; set; } = new(60f);
        public GenericValueModel<bool> Loop { get; set; } = new(true);
        public AssetSelectorModel AssetSelector { get; set; } = new();
        public Integer2Model Resolution { get; set; } = new(1920, 1080);
        public GenericValueModel<bool> UseCompositionResolution { get; set; } = new(false);
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
    }
}
