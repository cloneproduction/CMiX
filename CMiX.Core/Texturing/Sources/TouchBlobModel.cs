// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record TouchBlobModel : IControlModel, IPrefabModel, ITextureSourceModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2Model Resolution { get; set; } = new(1024, 1024);
        public GenericValueModel<bool> UseCompositionResolution { get; set; } = new(false);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Size { get; set; } = new(0.2f);
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
        public GenericValueModel<string> Color { get; set; } = new("#FFFFFF");
        public GenericValueModel<string> Background { get; set; } = new("#000000");
    }
}
