// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation
{
    public record ModulatableFloatModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public string Label { get; init; } = string.Empty;
        public GenericValueModel<float> Value { get; init; } = new(0f);
        public GenericValueModel<Guid?> ModulatorID { get; init; } = new(null);
        public GenericValueModel<string> BoundOutputName { get; init; } = new(null);
    }
}
