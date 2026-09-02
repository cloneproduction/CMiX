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
    //
    // ModulatorID/BoundOutputName are also exposed here (get-only, plain-typed rather than the
    // concrete GenericValue<T> field) so Modifier can walk any IModulatorBindable generically for
    // unassign-on-delete and resolve-on-load, the same way it already does for its own Channels -
    // without those two members it could only ever see Modulatable, not e.g. a ModifierModeSelector's
    // ModulatableCount.
    public interface IModulatorBindable
    {
        Guid? ModulatorID { get; }
        string BoundOutputName { get; }
        IModulator BoundModulator { get; }
        ICommand SetModulatorCommand { get; }

        // The one numeric type this field can be bound to - the assign popup filters a modulator's
        // Outputs down to only those whose ValueType matches this, so e.g. an int-only Count never
        // offers a float-only output. Fixed per implementer (Modulatable is always typeof(float),
        // ModulatableCount always typeof(int)), not something that varies per instance.
        System.Type RequiredValueType { get; }
    }
}
