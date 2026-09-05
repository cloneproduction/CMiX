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
                            ModulatableFloat translateX,
                            ModulatableFloat translateY,
                            ModulatableFloat translateZ,
                            ModulatableFloat scaleX,
                            ModulatableFloat scaleY,
                            ModulatableFloat scaleZ,
                            ModulatableFloat scaleUniform,
                            ModulatableFloat rotationX,
                            ModulatableFloat rotationY,
                            ModulatableFloat rotationZ,
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

            Bindables = new List<ModulatableFloat>
            {
                translateX, translateY, translateZ,
                scaleX, scaleY, scaleZ, scaleUniform,
                rotationX, rotationY, rotationZ
            };
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        public ModulatableFloat TranslateX => Bindables[0];
        public ModulatableFloat TranslateY => Bindables[1];
        public ModulatableFloat TranslateZ => Bindables[2];
        public ModulatableFloat ScaleX => Bindables[3];
        public ModulatableFloat ScaleY => Bindables[4];
        public ModulatableFloat ScaleZ => Bindables[5];
        public ModulatableFloat ScaleUniform => Bindables[6];
        public ModulatableFloat RotationX => Bindables[7];
        public ModulatableFloat RotationY => Bindables[8];
        public ModulatableFloat RotationZ => Bindables[9];

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
