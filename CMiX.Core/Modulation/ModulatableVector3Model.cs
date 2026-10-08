// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public record ModulatableVector3Model : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public ModulatableValueModel<float> X { get; init; } = ModulatableValueModel<float>.Of("X", 0f);
        public ModulatableValueModel<float> Y { get; init; } = ModulatableValueModel<float>.Of("Y", 0f);
        public ModulatableValueModel<float> Z { get; init; } = ModulatableValueModel<float>.Of("Z", 0f);

        public static ModulatableVector3Model Of(float x, float y, float z) => new()
        {
            X = ModulatableValueModel<float>.Of("X", x),
            Y = ModulatableValueModel<float>.Of("Y", y),
            Z = ModulatableValueModel<float>.Of("Z", z)
        };
    }
}
