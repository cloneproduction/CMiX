// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class LFOModel : IPrefabModel
    {
        public LFOModel()
        {
            Name = "LFO";
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            RandomizePhase = new GenericValueModel<float>(0.0f);
            BeatModifierManager = new PrefabManagerModel();
            DirectionXYZ = new DirectionXYZModel();
            PingPong = new GenericValueModel<bool>(false);
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            From = new GenericValueModel<float>(0.0f);
            To = new GenericValueModel<float>(1.0f);
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public string Name { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public GenericValueModel<float> RandomizePhase { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public GenericValueModel<float> From { get; set; }
        public GenericValueModel<float> To { get; set; }
        public DirectionXYZModel DirectionXYZ { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
    }
}
