// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
