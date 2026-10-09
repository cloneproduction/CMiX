// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public record AssetSelectorModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public GenericValueModel<string> FilePath { get; init; } = new(string.Empty);
        
    }
}
