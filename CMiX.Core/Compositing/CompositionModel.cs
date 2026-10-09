// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Compositing
{
    public record CompositionModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public GenericValueModel<Guid> SelectedOutputMappingID { get; init; } = new(Guid.Empty);
        public CompositingSettingsModel Compositing { get; init; } = new();
        public MaskSettingsModel Mask { get; init; } = new();
        public PrefabManagerModel TextureModifierManager { get; init; } = new();
        public PrefabManagerModel LayerManager { get; init; } = new();
        public PrefabManagerModel ModifierManager { get; init; } = new();
    }
}
