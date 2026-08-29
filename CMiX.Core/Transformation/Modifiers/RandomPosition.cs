// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.PositionModifier. No longer addable via
    // the picker; kept so already-saved Project data referencing RandomPosition still loads.
    public partial class RandomPosition : BeatModifiableModifierBase, ISpreadableModifier, IModifier
    {
        public RandomPosition(PrefabManager beatModifierManager,
                              PrefabService prefabService,
                              ModifierModeSelector modifierModeSelector,
                              Vector3 location)
            : base(prefabService, beatModifierManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Location = location;
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector3 Location { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomPositionModel
            {
                Location = (Vector3Model)Location.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomPositionModel)model;
            LoadBaseModel(m);
            Location.FromModel(m.Location);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
