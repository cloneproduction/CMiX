// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;

namespace CMiX.Core.Prefabs.Managers
{
    public record ManagerDataModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Collection<IControlModel> Items { get; set; } = new();
        public int SelectedIndex { get; set; }
    }
}
