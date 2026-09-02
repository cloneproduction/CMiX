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

        // Decides how any field this modulator gets bound to behaves - see ModulatorKind. A property
        // of the modulator itself so it stays consistent everywhere the same instance is plugged in,
        // rather than each bindable field guessing independently.
        ModulatorKind Kind { get; }

        // "Value" for a single-output modulator like BeatModifier; multiple names ("X", "Y") for
        // something that naturally produces more than one value together, e.g. a future tracking
        // modulator. Every existing modulator returns exactly one name, so nothing about today's
        // behavior changes until a real multi-output modulator exists.
        IReadOnlyList<string> OutputNames { get; }
    }
}
