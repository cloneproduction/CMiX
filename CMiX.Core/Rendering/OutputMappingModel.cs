// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Rendering
{
    public record OutputMappingModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public GenericValueModel<string> Name { get; init; } = new(string.Empty);
        public Integer2Model Resolution { get; init; } = new(1920, 1080);
        public GenericValueModel<TexcoordSemantic> TexcoordSemantic { get; init; } = new(Rendering.TexcoordSemantic.Texcoord0);
        public GenericValueModel<bool> Visibility { get; init; } = new(true);
    }
}
