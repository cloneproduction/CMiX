// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Animations
{
    // For the rare class that already spends its one inheritance slot elsewhere (e.g. a texture
    // filter that's also beat-modifiable, like LFOUV/RandomUV) and so can't inherit
    // BeatModifiableModifierBase alongside it. Any IBeatModifiable gets the same
    // Dispose/load-manager behavior through these instead of a base class.
    public static class BeatModifiableExtensions
    {
        public static void DisposeBeatModifier(this IBeatModifiable self) =>
            self.BeatModifierManager.Dispose();

        public static void LoadBeatModifier(this IBeatModifiable self, PrefabManagerModel model) =>
            LoadManager(self.BeatModifierManager, model);
    }
}
