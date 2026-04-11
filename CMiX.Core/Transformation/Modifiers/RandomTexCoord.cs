// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomTexCoord : ObservableObject, IBeatModifiable, ISpreadableModifier
    {
        public RandomTexCoord(PrefabService prefabService,
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

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RandomTexCoordModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            Location = (Vector2Model)Location.ToModel(),
            Scale = (Vector2Model)Scale.ToModel(),
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            Rotation = (GenericValueModel<float>)Rotation.ToModel(),
            SamplerState = (SamplerStateModel)SamplerState.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomTexCoordModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Uniform.FromModel(m.Uniform);
            Rotation.FromModel(m.Rotation);
            SamplerState.FromModel(m.SamplerState);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
    }
}
