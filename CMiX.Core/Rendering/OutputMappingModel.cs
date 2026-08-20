// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
