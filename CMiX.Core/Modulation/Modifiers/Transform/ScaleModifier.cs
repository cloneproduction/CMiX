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
    public partial class ScaleModifier : Modifier, ISpreadableModifier
    {
        public ScaleModifier(PrefabService prefabService,
                             PrefabManager modulatorManager,
                             ControlRepository controlRepository,
                             ModifierModeSelector modifierModeSelector,
                             ModulatableValue<float> bindableX,
                             ModulatableValue<float> bindableY,
                             ModulatableValue<float> bindableZ,
                             ModulatableValue<float> bindableUniform)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { bindableX, bindableY, bindableZ, bindableUniform };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];
        public ModulatableValue<float> Z => Bindables[2];

        public ModulatableValue<float> Uniform => Bindables[3];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ScaleModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel(),
                Z = (ModulatableValueModel<float>)Z.ToModel(),
                Uniform = (ModulatableValueModel<float>)Uniform.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ScaleModifierModel)model;
            LoadBaseModel(m);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
            Uniform.FromModel(m.Uniform);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
