// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public record LayerModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public CompositingSettingsModel LayerSettings { get; init; } = new();
        public MaskSettingsModel LayerMaskSettings { get; init; } = new();
        public PrefabManagerModel TextureModifierManager { get; init; } = new();
        public PrefabManagerModel ModifierManager { get; init; } = new();
        public AmbientOcclusionModel AmbientOcclusion { get; init; } = new();
        public LocalReflectionModel LocalReflection { get; init; } = new();
        public PrefabManagerModel ModelEntityManager { get; init; } = new();
        public PrefabManagerModel CameraManager { get; init; } = new();
        public PrefabManagerModel LightManager { get; init; } = new();
    }
}
