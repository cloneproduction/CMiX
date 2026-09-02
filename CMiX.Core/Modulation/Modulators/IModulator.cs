// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public interface IModulator : IPrefab
    {
        // UI-hover state, mirroring how Modifier/BeatModifiableModifierBase already carry
        // IsExpanded - lets any channel bound to this modulator light up while its own box (in
        // the modulator stack list) is hovered, without CMiX.Core knowing anything about how
        // that's drawn.
        bool IsHovered { get; set; }

        // ModifierPanel (the shared per-item wrapper for both Modifier and Modulator list rows)
        // binds its Expander to DataContext.IsExpanded unconditionally, so every IModulator needs
        // one too, matching Modifier's own IsExpanded.
        bool IsExpanded { get; set; }

        // One entry for a single-output modulator like BeatModulator; multiple entries ("X", "Y")
        // for something that naturally produces more than one value together, e.g. TrackingModulator.
        // Kind (Set/Modulate) and ValueType (Integer/Float) live per-entry here rather than flat on
        // IModulator - a single modulator can produce outputs of different kinds/types at once, so
        // neither can be a single property of the modulator as a whole.
        IReadOnlyList<ModulatorOutput> Outputs { get; }
    }
}
