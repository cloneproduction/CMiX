// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Rendering
{
    public record OutputMappingManagerModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public List<OutputMappingModel> Items { get; init; } = new();
    }
}
