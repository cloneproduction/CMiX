// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs.Managers
{
    public record CollectionManagerModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ManagerDataModel ManagerData { get; init; } = new();
    }
}
