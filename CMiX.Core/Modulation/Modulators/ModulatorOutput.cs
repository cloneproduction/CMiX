// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation.Modulators
{
    // One named value a modulator produces. Kind lives here rather than flat on IModulator because
    // a single modulator can legitimately produce both Set and Modulate outputs at once (e.g. a
    // future BeatModulator.CycleCount alongside its existing Value) - so "what happens to a bound
    // field" has to be decided per-output, not per-modulator.
    public record ModulatorOutput(string Name, ModulatorKind Kind, ModulatorValueType ValueType);
}
