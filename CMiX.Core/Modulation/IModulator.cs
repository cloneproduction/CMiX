// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation
{
    public interface IModulator : IPrefab
    {
        // UI-hover state, mirroring how Modifier/BeatModifiableModifierBase already carry
        // IsExpanded - lets any channel bound to this modulator light up while its own box (in
        // the modulator stack list) is hovered, without CMiX.Core knowing anything about how
        // that's drawn.
        bool IsHovered { get; set; }
    }
}
