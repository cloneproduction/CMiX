// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            DirectionXYZ = directionXYZ;
            Mode = mode;

            translateX.Label = "X";
            translateY.Label = "Y";
            translateZ.Label = "Z";
            scaleX.Label = "X";
            scaleY.Label = "Y";
            scaleZ.Label = "Z";
            scaleUniform.Label = "Uniform";
            rotationX.Label = "X";
            rotationY.Label = "Y";
            rotationZ.Label = "Z";

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
                Mode = (GenericValueModel<ModifierMode>)Mode.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformSRTModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Mode.FromModel(m.Mode);
        }
    }
}
