// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class CircularSpreadModifier : Modifier, ISpreadableModifier
    {
        public CircularSpreadModifier(PrefabService prefabService,
                                      PrefabManager modulatorManager,
                                      ControlRepository controlRepository,
                                      ModifierModeSelector modifierModeSelector,
                                      ModulatableValue<float> widthX,
                                      ModulatableValue<float> widthY,
                                      ModulatableValue<float> phase,
                                      ModulatableValue<float> factor)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { widthX, widthY, phase, factor };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];
        public ModulatableValue<float> Phase => Bindables[2];
        public ModulatableValue<float> Factor => Bindables[3];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CircularSpreadModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel(),
                Phase = (ModulatableValueModel<float>)Phase.ToModel(),
                Factor = (ModulatableValueModel<float>)Factor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CircularSpreadModifierModel)model;
            LoadBaseModel(m);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Phase.FromModel(m.Phase);
            Factor.FromModel(m.Factor);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
