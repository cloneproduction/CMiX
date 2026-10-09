// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record VideoInModel : IControlModel, IPrefabModel, ITextureSourceModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
        public GenericValueModel<int> SizeX { get; set; } = new(1920);
        public GenericValueModel<int> SizeY { get; set; } = new(1080);
        public GenericValueModel<bool> UseCompositionResolution { get; set; } = new(false);
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
