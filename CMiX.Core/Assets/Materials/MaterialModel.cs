// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Materials
{
    public record MaterialModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; } = new();
        public MaskTextureModel MaskTexture { get; set; } = new();
        public MaterialSettingsModel MaterialSettings { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel ExplodeTriangleTextureManager { get; set; } = new();
        public GenericValueModel<float> ExplodeStrength { get; set; } = new(0.6f);
        public PrefabManagerModel ModifierManager { get; set; } = new();
    }
}
