// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableVector3
    {
        public ModulatableVector3(ModulatableValue<float> x, ModulatableValue<float> y, ModulatableValue<float> z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public ModulatableValue<float> X { get; }
        public ModulatableValue<float> Y { get; }
        public ModulatableValue<float> Z { get; }
    }
}
