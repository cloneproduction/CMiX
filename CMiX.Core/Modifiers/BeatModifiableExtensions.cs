// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modifiers
{
    public static class BeatModifiableExtensions
    {
        public static void DisposeBeatModifier(this IBeatModifiable self) =>
            self.BeatModifierManager.Dispose();

        public static void LoadBeatModifier(this IBeatModifiable self, PrefabManagerModel model) =>
            LoadManager(self.BeatModifierManager, model);
    }
}
