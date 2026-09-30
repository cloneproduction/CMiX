// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public record ModulatableVector2Model : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public ModulatableValueModel<float> X { get; init; } = new();
        public ModulatableValueModel<float> Y { get; init; } = new();
    }
}
