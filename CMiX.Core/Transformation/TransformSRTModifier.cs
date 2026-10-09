// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Transformation
{
    [ModifierPanel(typeof(Entity))]
    public partial class TransformSRTModifier : Modifier, ISpreadableModifier
    {
        public TransformSRTModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ControlRepository controlRepository,
                            ModifierModeSelector modifierModeSelector,
                            ModulatableValue<float> translateX,
                            ModulatableValue<float> translateY,
                            ModulatableValue<float> translateZ,
                            ModulatableValue<float> scaleX,
                            ModulatableValue<float> scaleY,
                            ModulatableValue<float> scaleZ,
                            ModulatableValue<float> scaleUniform,
                            ModulatableValue<float> rotationX,
                            ModulatableValue<float> rotationY,
                            ModulatableValue<float> rotationZ,
                            DirectionXYZ directionXYZ,
                            GenericValue<ModifierMode> mode)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            DirectionXYZ = directionXYZ;
            Mode = mode;

            Bindables = new List<ModulatableValue<float>>
            {
                translateX, translateY, translateZ,
                scaleX, scaleY, scaleZ, scaleUniform,
                rotationX, rotationY, rotationZ
            };
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        public ModulatableValue<float> TranslateX => Bindables[0];
        public ModulatableValue<float> TranslateY => Bindables[1];
        public ModulatableValue<float> TranslateZ => Bindables[2];
        public ModulatableValue<float> ScaleX => Bindables[3];
        public ModulatableValue<float> ScaleY => Bindables[4];
        public ModulatableValue<float> ScaleZ => Bindables[5];
        public ModulatableValue<float> ScaleUniform => Bindables[6];
        public ModulatableValue<float> RotationX => Bindables[7];
        public ModulatableValue<float> RotationY => Bindables[8];
        public ModulatableValue<float> RotationZ => Bindables[9];

        public override IControlModel ToModel()
        {
            var model = new TransformSRTModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
                Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
                TranslateX = (ModulatableValueModel<float>)TranslateX.ToModel(),
                TranslateY = (ModulatableValueModel<float>)TranslateY.ToModel(),
                TranslateZ = (ModulatableValueModel<float>)TranslateZ.ToModel(),
                ScaleX = (ModulatableValueModel<float>)ScaleX.ToModel(),
                ScaleY = (ModulatableValueModel<float>)ScaleY.ToModel(),
                ScaleZ = (ModulatableValueModel<float>)ScaleZ.ToModel(),
                ScaleUniform = (ModulatableValueModel<float>)ScaleUniform.ToModel(),
                RotationX = (ModulatableValueModel<float>)RotationX.ToModel(),
                RotationY = (ModulatableValueModel<float>)RotationY.ToModel(),
                RotationZ = (ModulatableValueModel<float>)RotationZ.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformSRTModifierModel)model;
            LoadBaseModel(m);
            TranslateX.FromModel(m.TranslateX);
            TranslateY.FromModel(m.TranslateY);
            TranslateZ.FromModel(m.TranslateZ);
            ScaleX.FromModel(m.ScaleX);
            ScaleY.FromModel(m.ScaleY);
            ScaleZ.FromModel(m.ScaleZ);
            ScaleUniform.FromModel(m.ScaleUniform);
            RotationX.FromModel(m.RotationX);
            RotationY.FromModel(m.RotationY);
            RotationZ.FromModel(m.RotationZ);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Mode.FromModel(m.Mode);
        }
    }
}
