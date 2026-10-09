// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class LinearModifier : Modifier, ISpreadableModifier
    {
        public LinearModifier(PrefabService prefabService,
                                 PrefabManager modulatorManager,
                                 ControlRepository controlRepository,
                                 ModifierModeSelector modifierModeSelector,
                                 ModulatableValue<float> width,
                                 ModulatableValue<float> phase)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { width, phase };
        }

        public ModulatableValue<float> Width => Bindables[0];
        public ModulatableValue<float> Phase => Bindables[1];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LinearModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Width = (ModulatableValueModel<float>)Width.ToModel(),
                Phase = (ModulatableValueModel<float>)Phase.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LinearModifierModel)model;
            LoadBaseModel(m);
            Width.FromModel(m.Width);
            Phase.FromModel(m.Phase);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
