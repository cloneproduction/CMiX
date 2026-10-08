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
                              ControlRepository controlRepository,
                              ModifierModeSelector modifierModeSelector,
                              ModulatableValue<float> seed,
                              ModulatableValue<float> center,
                              ModulatableValue<float> width)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { seed, center, width };
        }

        public ModulatableValue<float> Seed => Bindables[0];
        public ModulatableValue<float> Center => Bindables[1];
        public ModulatableValue<float> Width => Bindables[2];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Seed = (ModulatableValueModel<float>)Seed.ToModel(),
                Center = (ModulatableValueModel<float>)Center.ToModel(),
                Width = (ModulatableValueModel<float>)Width.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomModifierModel)model;
            LoadBaseModel(m);
            Seed.FromModel(m.Seed);
            Center.FromModel(m.Center);
            Width.FromModel(m.Width);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
