// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class Flip : ObservableObject, IModifier, IBeatModifiable, IDisposable
    {
        public Flip(PrefabService prefabService,
                    PrefabManager beatModifierManager,
                    DirectionXYZ directionXYZ)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
            DirectionXYZ = directionXYZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new FlipModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (FlipModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            DirectionXYZ.FromModel(m.DirectionXYZ);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.Dispose();
        }
    }
}
