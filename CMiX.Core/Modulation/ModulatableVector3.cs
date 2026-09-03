// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableVector3
    {
        public ModulatableVector3(ModulatableFloat x, ModulatableFloat y, ModulatableFloat z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public ModulatableFloat X { get; }
        public ModulatableFloat Y { get; }
        public ModulatableFloat Z { get; }
    }
}
