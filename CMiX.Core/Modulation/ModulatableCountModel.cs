// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation
{
    public record ModulatableCountModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public string Label { get; init; } = string.Empty;
        public GenericValueModel<int> Value { get; init; } = new(1);
        public GenericValueModel<Guid?> ModulatorID { get; init; } = new(null);
        public GenericValueModel<string> BoundOutputName { get; init; } = new(null);
    }
}
