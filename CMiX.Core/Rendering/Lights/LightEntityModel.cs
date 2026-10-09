// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Rendering.Lights
{
    public record LightEntityModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public TransformSRTModel TransformSRT { get; set; } = new();
        public LightSettingsModel Settings { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabManagerModel ColorPaletteManager { get; set; } = new();
    }
}
