// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Compositing
{
    public record ColorModel() : GenericValueModel<string>("#FFFFFFFF"), IPrefabModel
    {
        public PrefabServiceModel PrefabService { get; init; } = new();
    }
}
