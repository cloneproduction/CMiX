// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class TexCoordModifier : Modifier, ISpreadableModifier
    {
        public TexCoordModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ModifierModeSelector modifierModeSelector,
                                SamplerState samplerState,
                                ModulatableFloat locationX, ModulatableFloat locationY,
                                ModulatableFloat scaleX, ModulatableFloat scaleY,
                                ModulatableFloat rotation,
                                ModulatableFloat uniform)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            SamplerState = samplerState;
            Location = new ModulatableVector2(locationX, locationY);
            Scale = new ModulatableVector2(scaleX, scaleY);
            rotation.Label = "Rotation";
            uniform.Label = "Uniform";
            Bindables = new List<ModulatableFloat> { locationX, locationY, scaleX, scaleY, rotation, uniform };
        }

        public ModulatableVector2 Location { get; }
        public ModulatableVector2 Scale { get; }

        public ModulatableFloat Rotation => Bindables[4];
        public ModulatableFloat Uniform => Bindables[5];

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public SamplerState SamplerState { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TexCoordModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                SamplerState = (SamplerStateModel)SamplerState.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TexCoordModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            SamplerState.FromModel(m.SamplerState);
        }
    }
}
