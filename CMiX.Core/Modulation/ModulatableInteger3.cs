// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableInteger3
    {
        public ModulatableInteger3(ModulatableValue<int> x, ModulatableValue<int> y, ModulatableValue<int> z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public ModulatableValue<int> X { get; }
        public ModulatableValue<int> Y { get; }
        public ModulatableValue<int> Z { get; }

        public IEnumerable<IModulatorBindable> Bindables => new IModulatorBindable[] { X, Y, Z };
    }
}
