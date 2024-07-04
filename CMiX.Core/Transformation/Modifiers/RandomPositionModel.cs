// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomPositionModel : IControlModel, IPrefabModel
    {
        public RandomPositionModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Location = new Vector3Model();
            ModifierModeSelector = new ModifierModeSelectorModel();
            BeatModifierManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Vector3Model Location { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
