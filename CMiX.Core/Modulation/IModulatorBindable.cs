// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Core.Modulation
{
    public interface IModulatorBindable
    {
        Guid? ModulatorID { get; }
        string BoundOutputName { get; }
        IModulator BoundModulator { get; }
        bool IsModulated { get; }
        ICommand SetModulatorCommand { get; }
        Func<Guid, IModulator> ModulatorLookup { get; set; }

        bool CanBind(IModulatorOutput output);
    }
}
