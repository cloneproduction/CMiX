// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Core.Modulation
{
    // The shape ModulatorAssignableUserControl needs from whatever its DataContext is - introduced
    // so that shape can be checked directly instead of against the concrete Modulatable type, which
    // would otherwise silently fail once a second implementer (e.g. a future int-valued counterpart)
    // needs the same assign-popup machinery.
    public interface IModulatorBindable
    {
        IModulator BoundModulator { get; }
        ICommand SetModulatorCommand { get; }
    }
}
