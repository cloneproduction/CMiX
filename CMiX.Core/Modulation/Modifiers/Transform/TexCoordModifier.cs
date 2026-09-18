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
                                ControlRepository controlRepository,
                                ModifierModeSelector modifierModeSelector,
                                SamplerState samplerState,
                                ModulatableValue<float> locationX, ModulatableValue<float> locationY,
                                ModulatableValue<float> scaleX, ModulatableValue<float> scaleY,
                                ModulatableValue<float> rotation,
                                ModulatableValue<float> uniform)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            SamplerState = samplerState;
            Transform = new TexCoordTransform(locationX, locationY, scaleX, scaleY, rotation, uniform);
            Bindables = Transform.Bindables;
        }

        public TexCoordTransform Transform { get; }
        public ModulatableVector2 Location => Transform.Location;
        public ModulatableVector2 Scale => Transform.Scale;
        public ModulatableValue<float> Rotation => Transform.Rotation;
        public ModulatableValue<float> Uniform => Transform.Uniform;

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
