// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation
{
    public record ChannelBindingModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Guid? ModulatorID { get; init; }
        public GenericValueModel<float> Depth { get; init; } = new(0f);
    }
}
