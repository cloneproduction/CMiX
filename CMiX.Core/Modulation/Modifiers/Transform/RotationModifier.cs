// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
    public partial class RotationModifier : Modifier, ISpreadableModifier
    {
        public RotationModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ControlRepository controlRepository,
                                ModifierModeSelector modifierModeSelector,
                                ModulatableValue<float> bindableX,
                                ModulatableValue<float> bindableY,
                                ModulatableValue<float> bindableZ)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { bindableX, bindableY, bindableZ };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];
        public ModulatableValue<float> Z => Bindables[2];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RotationModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel(),
                Z = (ModulatableValueModel<float>)Z.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RotationModifierModel)model;
            LoadBaseModel(m);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
