// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation.Modulators
{
    // What kind of number an output produces, and what kind of number a bindable field accepts -
    // the assign popup only offers an output whose ValueType matches the field's own
    // IModulatorBindable.RequiredValueType. Deliberately strict (no implicit Integer<->Float
    // conversion): loosening this to let a field accept both would readmit exactly the outputs it
    // was meant to exclude, since two unrelated outputs can share the same numeric type.
    public enum ModulatorValueType
    {
        Integer,
        Float
    }
}
