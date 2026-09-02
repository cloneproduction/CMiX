// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation
{
    // One shared ModulatorManager for all 10 channels (Translate/Scale/Rotation X/Y/Z plus Scale's
    // Uniform) - every channel picks from the same modulator list. One shared ModifierModeSelector
    // too, matching every other Modulation modifier (ScaleModifier, RotationModifier,
    // TranslateModifier) - one Modifier = one ModifierModeSelector, regardless of channel count.
    [ModifierPanel(typeof(Entity))]
    public partial class TransformSRTModifier : Modifier
    {
        public TransformSRTModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ModifierModeSelector modifierModeSelector,
                            Modulatable translateX,
                            Modulatable translateY,
                            Modulatable translateZ,
                            Modulatable scaleX,
                            Modulatable scaleY,
                            Modulatable scaleZ,
                            Modulatable scaleUniform,
                            Modulatable rotationX,
                            Modulatable rotationY,
                            Modulatable rotationZ,
                            DirectionXYZ directionXYZ,
                            GenericValue<ModifierMode> mode)
            : base(prefabService, modulatorManager)
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

            Channels = new List<Modulatable>
            {
                translateX, translateY, translateZ,
                scaleX, scaleY, scaleZ, scaleUniform,
                rotationX, rotationY, rotationZ
            };
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        // Reaches ModifierModeSelector's own bindable Count for unassign-on-delete/resolve-on-load -
        // see Modifier.AdditionalModulatorBindables.
        protected override IEnumerable<IModulatorBindable> AdditionalModulatorBindables => new IModulatorBindable[] { ModifierModeSelector.Count };

        // Convenience accessors into Channels, purely for the view's bindings - Channels itself
        // stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable TranslateX => Channels[0];
        public Modulatable TranslateY => Channels[1];
        public Modulatable TranslateZ => Channels[2];
        public Modulatable ScaleX => Channels[3];
        public Modulatable ScaleY => Channels[4];
        public Modulatable ScaleZ => Channels[5];
        public Modulatable ScaleUniform => Channels[6];
        public Modulatable RotationX => Channels[7];
        public Modulatable RotationY => Channels[8];
        public Modulatable RotationZ => Channels[9];

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
            ResolveModulatorBinding(ModifierModeSelector.Count);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Mode.FromModel(m.Mode);
        }
    }
}
