// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableVector2
    {
        public ModulatableVector2(ModulatableFloat x, ModulatableFloat y)
        {
            x.Label = "X";
            y.Label = "Y";
            X = x;
            Y = y;
        }

        public ModulatableFloat X { get; }
        public ModulatableFloat Y { get; }
    }
}
