// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modulation.Modulators;

namespace CMiX.Core.Modulation
{
    // The actual DataContext of a selectable row in the assign popup, replacing a bare IModulator.
    // For a single-output modulator (every modulator today) there's still exactly one row, with
    // Output set to that modulator's one entry, so nothing about today's rendering or binding
    // behavior changes. For a multi-output modulator, one instance of this exists per entry in
    // Outputs, letting several channels bind to the same modulator instance while each picks its
    // own named output. Carrying the whole ModulatorOutput (not just its Name) means Kind/ValueType
    // are available wherever a selection is, e.g. to a bindable field's SetModulator handler.
    public record ModulatorOutputSelection(IModulator Modulator, ModulatorOutput Output);
}
