// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Discoverable on Entity, matching the old RandomXYZ's own scope exactly - both stay addable
    // side by side until RandomXYZ is confirmed superseded by a live VL check, per this session's
    // migration approach. RandomXYZ is not touched by this change. ModifierModeSelector, Gaussian,
    // and the three RandomizeX/Y/Z toggle flags are ported as-is (non-modulatable); Location,
    // Scale, and Rotation all become independent, modulatable XYZ groups sharing this Modifier's
    // one modulator stack. RandomXYZModel also carries an orphan Uniform field the old ViewModel
    // never reads or writes (dead, like RandomPosition.axaml's stale RandomizeLocation binding
    // found earlier this migration) - not ported.
    [ModifierPanel(typeof(Entity))]
    public partial class XYZModifier : Modifier
    {
        public XYZModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<bool> gaussian,
                           GenericValue<bool> randomizeLocation,
                           Channel locationX, Channel locationY, Channel locationZ,
                           GenericValue<bool> randomizeScale,
                           Channel scaleX, Channel scaleY, Channel scaleZ,
                           GenericValue<bool> randomizeRotation,
                           Channel rotationX, Channel rotationY, Channel rotationZ)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Gaussian = gaussian;
            RandomizeLocation = randomizeLocation;
            RandomizeScale = randomizeScale;
            RandomizeRotation = randomizeRotation;
            Location = new ChannelGroup(locationX, locationY, locationZ);
            Scale = new ChannelGroup(scaleX, scaleY, scaleZ);
            Rotation = new ChannelGroup(rotationX, rotationY, rotationZ);
            Channels = new List<Channel>
            {
                locationX, locationY, locationZ,
                scaleX, scaleY, scaleZ,
                rotationX, rotationY, rotationZ
            };
        }

        // Each group is bound by its own ChannelVectorXYZ in the view (via DataContext), all
        // sharing this Modifier's single ModulatorManager (set explicitly on each usage, not
        // inherited from DataContext) - see ChannelVectorXYZ.axaml.cs.
        public ChannelGroup Location { get; }
        public ChannelGroup Scale { get; }
        public ChannelGroup Rotation { get; }

        // Non-modulatable, ported as-is from RandomXYZ for one-to-one field parity.
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
            Gaussian.FromModel(m.Gaussian);
            RandomizeLocation.FromModel(m.RandomizeLocation);
            RandomizeScale.FromModel(m.RandomizeScale);
            RandomizeRotation.FromModel(m.RandomizeRotation);
        }
    }
}
