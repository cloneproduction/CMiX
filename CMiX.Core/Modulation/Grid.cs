// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Modulation
{
    // Renamed from GridModifier to bare Grid, claiming the name freed up by the old Grid (now
    // GridLegacy) so VL's exact-name matching can target this class directly. One-to-one field
    // parity with the old Grid is complete: Count and ModifierModeSelector are ported as-is
    // (non-modulatable); Width and Phase become two independent, modulatable XYZ groups sharing
    // this Modifier's one modulator stack - new capability the old Grid never had.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class Grid : Modifier
    {
        public Grid(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ModifierModeSelector modifierModeSelector,
                            Integer3 count,
                            Channel widthX, Channel widthY, Channel widthZ,
                            Channel phaseX, Channel phaseY, Channel phaseZ)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Count = count;
            Width = new ChannelGroup(widthX, widthY, widthZ);
            Phase = new ChannelGroup(phaseX, phaseY, phaseZ);
            Channels = new List<Channel> { widthX, widthY, widthZ, phaseX, phaseY, phaseZ };
        }

        // Each group is bound by its own ChannelVectorXYZ in the view (via DataContext), both
        // sharing this Modifier's single ModulatorManager (set explicitly on each usage, not
        // inherited from DataContext) - see ChannelVectorXYZ.axaml.cs.
        public ChannelGroup Width { get; }
        public ChannelGroup Phase { get; }

        // Non-modulatable, ported as-is from Grid for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Integer3 Count { get; set; }

        public override IControlModel ToModel()
        {
            var model = new GridModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                Count = (Integer3Model)Count.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (GridModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            Count.FromModel(m.Count);
        }
    }
}
