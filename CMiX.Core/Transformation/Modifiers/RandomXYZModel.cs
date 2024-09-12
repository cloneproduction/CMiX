// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomXYZModel : IPrefabModel
    {
        public RandomXYZModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Gaussian = new GenericValueModel<bool>(false);
            BeatModifierManager = new PrefabManagerModel();
            RandomizeLocation = new GenericValueModel<bool>(true);
            Location = new Vector3Model();
            RandomizeScale = new GenericValueModel<bool>(true);
            Uniform = new GenericValueModel<float>();
            Scale = new Vector3Model();
            RandomizeRotation = new GenericValueModel<bool>(true);
            Rotation = new Vector3Model(0.0f, 0.0f, 0.0f);
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> Gaussian { get; set; }
        public GenericValueModel<bool> RandomizeLocation { get; set; }
        public Vector3Model Location { get; set; }
        public GenericValueModel<bool> RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }
        public GenericValueModel<bool> RandomizeRotation { get; set; }
        public Vector3Model Rotation { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
