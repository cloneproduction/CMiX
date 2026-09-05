// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class RandomModifier : Modifier, ISpreadableModifier
    {
        public RandomModifier(PrefabService prefabService,
                              PrefabManager modulatorManager,
                              ModifierModeSelector modifierModeSelector,
                              GenericValue<int> seed,
                              ModulatableFloat center,
                              ModulatableFloat width)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Seed = seed;
            center.Label = "Center";
            width.Label = "Width";
            width.Value.Value = 1.0f;
            Bindables = new List<ModulatableFloat> { center, width };
        }

        public ModulatableFloat Center => Bindables[0];
        public ModulatableFloat Width => Bindables[1];

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<int> Seed { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Seed = (GenericValueModel<int>)Seed.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            Seed.FromModel(m.Seed);
        }
    }
}
