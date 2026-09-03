// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class XYZModifier : Modifier, ISpreadableModifier
    {
        public XYZModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<bool> gaussian,
                           GenericValue<bool> randomizeLocation,
                           ModulatableFloat locationX, ModulatableFloat locationY, ModulatableFloat locationZ,
                           GenericValue<bool> randomizeScale,
                           ModulatableFloat scaleX, ModulatableFloat scaleY, ModulatableFloat scaleZ,
                           GenericValue<bool> randomizeRotation,
                           ModulatableFloat rotationX, ModulatableFloat rotationY, ModulatableFloat rotationZ)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            Gaussian = gaussian;
            RandomizeLocation = randomizeLocation;
            RandomizeScale = randomizeScale;
            RandomizeRotation = randomizeRotation;
            Location = new ModulatableVector3(locationX, locationY, locationZ);
            Scale = new ModulatableVector3(scaleX, scaleY, scaleZ);
            Rotation = new ModulatableVector3(rotationX, rotationY, rotationZ);
            Bindables = new List<ModulatableFloat>
            {
                locationX, locationY, locationZ,
                scaleX, scaleY, scaleZ,
                rotationX, rotationY, rotationZ
            };
        }

        public ModulatableVector3 Location { get; }
        public ModulatableVector3 Scale { get; }
        public ModulatableVector3 Rotation { get; }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<bool> Gaussian { get; set; }
        public GenericValue<bool> RandomizeLocation { get; set; }
        public GenericValue<bool> RandomizeScale { get; set; }
        public GenericValue<bool> RandomizeRotation { get; set; }

        public override IControlModel ToModel()
        {
            var model = new XYZModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Gaussian = (GenericValueModel<bool>)Gaussian.ToModel(),
                RandomizeLocation = (GenericValueModel<bool>)RandomizeLocation.ToModel(),
                RandomizeScale = (GenericValueModel<bool>)RandomizeScale.ToModel(),
                RandomizeRotation = (GenericValueModel<bool>)RandomizeRotation.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (XYZModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            Gaussian.FromModel(m.Gaussian);
            RandomizeLocation.FromModel(m.RandomizeLocation);
            RandomizeScale.FromModel(m.RandomizeScale);
            RandomizeRotation.FromModel(m.RandomizeRotation);
        }
    }
}
