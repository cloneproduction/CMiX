// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public record BillboardModifierModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new PrefabServiceModel();
    }
}
