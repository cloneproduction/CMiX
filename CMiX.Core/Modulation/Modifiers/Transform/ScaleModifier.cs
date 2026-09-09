// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
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
                             ModifierModeSelector modifierModeSelector,
                             ModulatableValue<float> bindableX,
                             ModulatableValue<float> bindableY,
                             ModulatableValue<float> bindableZ,
                             ModulatableValue<float> bindableUniform)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Bindables = new List<ModulatableValue<float>> { bindableX, bindableY, bindableZ, bindableUniform };
            var labels = new[] { "X", "Y", "Z", "Uniform" };
            for (int i = 0; i < Bindables.Count; i++)
            {
                Bindables[i].Label = labels[i];
                Bindables[i].Value = 1f;
            }
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
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ScaleModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
