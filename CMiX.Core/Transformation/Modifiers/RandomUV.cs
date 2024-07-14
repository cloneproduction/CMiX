// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomUV : IPrefab, IBeatModifiable, ISpreadableModifier
    {
        public RandomUV(PrefabService prefabService,
                        ModifierModeSelector modifierModeSelector,
                        PrefabManager beatModifierManager,
                        SamplerState samplerState,
                        Vector2 location,
                        Vector2 scale,
                        GenericValue<float> rotation,
                        GenericValue<float> uniform)
        {
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            BeatModifierManager = beatModifierManager;
            SamplerState = samplerState;
            Location = location;
            Scale = scale;
            Rotation = rotation;
            Uniform = uniform;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        public Vector2 Location { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }
    }
}
