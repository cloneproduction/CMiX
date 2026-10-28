// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
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
                                ModulatableValue<float> locationX, 
                                ModulatableValue<float> locationY,
                                ModulatableValue<float> scaleX, 
                                ModulatableValue<float> scaleY,
                                ModulatableValue<float> rotation,
                                ModulatableValue<float> uniform)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Transform = new TexCoordTransform(locationX, locationY, scaleX, scaleY, rotation, uniform);
            Bindables = Transform.Bindables;
        }

        public TexCoordTransform Transform { get; }
        public ModulatableVector2 Location => Transform.Location;
        public ModulatableVector2 Scale => Transform.Scale;
        public ModulatableValue<float> Rotation => Transform.Rotation;
        public ModulatableValue<float> Uniform => Transform.Uniform;

        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TexCoordModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Location = (ModulatableVector2Model)Location.ToModel(),
                Scale = (ModulatableVector2Model)Scale.ToModel(),
                Rotation = (ModulatableValueModel<float>)Rotation.ToModel(),
                Uniform = (ModulatableValueModel<float>)Uniform.ToModel(),
                SamplerState = UnusedSamplerState()
            };
            PopulateBaseModel(model);
            return model;
        }

        // The model has a sampler state that this modifier does not use. It gets ids that stay the same.
        private SamplerStateModel UnusedSamplerState()
        {
            Guid IdOf(string name) => DerivedId.Of(ID, nameof(TexCoordModifierModel.SamplerState) + "." + name);

            return new SamplerStateModel
            {
                ID = IdOf(nameof(SamplerStateModel.ID)),
                BorderColor = new("#FFFF00FF") { ID = IdOf(nameof(SamplerStateModel.BorderColor)) },
                AddressU = new(TextureAddressMode.Mirror) { ID = IdOf(nameof(SamplerStateModel.AddressU)) },
                AddressV = new(TextureAddressMode.Mirror) { ID = IdOf(nameof(SamplerStateModel.AddressV)) }
            };
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TexCoordModifierModel)model;
            LoadBaseModel(m);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Rotation.FromModel(m.Rotation);
            Uniform.FromModel(m.Uniform);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
        }
    }
}
