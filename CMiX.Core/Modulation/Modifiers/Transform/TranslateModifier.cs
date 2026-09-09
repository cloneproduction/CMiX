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
    public partial class TranslateModifier : Modifier, ISpreadableModifier
    {
        public TranslateModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ModifierModeSelector modifierModeSelector,
                                ModulatableValue<float> bindableX,
                                ModulatableValue<float> bindableY,
                                ModulatableValue<float> bindableZ)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            bindableX.Label = "X";
            bindableY.Label = "Y";
            bindableZ.Label = "Z";
            Bindables = new List<ModulatableValue<float>> { bindableX, bindableY, bindableZ };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];
        public ModulatableValue<float> Z => Bindables[2];

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TranslateModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TranslateModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
