// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation.Modulators
{
    // Decides how a bound field behaves, uniformly wherever this modulator gets plugged in - not
    // something each field (ModulatableFloat, ModulatableInteger, ...) hardcodes for itself.
    public enum ModulatorKind
    {
        // The field's own typed number stays meaningful and editable - reinterpreted as a base/depth
        // the modulator's output blends around, e.g. BeatModulator randomizing near a typed value.
        Modulate,

        // The field's typed number is meaningless once bound - the modulator's output replaces it
        // entirely, so the field locks read-only, e.g. a live tracked headcount.
        Set
    }
}
