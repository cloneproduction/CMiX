// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Text.Modifiers
{
    [ModifierPanel(typeof(TextEntity))]
    public partial class CharWriter : ObservableObject, IPrefab, IBeatModifiable, IDisposable
    {
        public CharWriter(PrefabService prefabService,
                          PrefabManager beatModifierManager)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new CharWriterModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CharWriterModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.Dispose();
        }
    }
}
