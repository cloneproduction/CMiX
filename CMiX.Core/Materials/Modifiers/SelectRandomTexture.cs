// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Materials.Modifiers
{
    [ModifierPanel(typeof(Material))]
    public partial class SelectRandomTexture : ObservableObject, IBeatModifiable, IModifier, IDisposable
    {
        public SelectRandomTexture(PrefabManager beatModifierManager,
                                  PrefabService prefabService,
                                  GenericValue<TextureFrom> textureFrom)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            TextureFrom = textureFrom;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public GenericValue<TextureFrom> TextureFrom { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new SelectRandomTextureModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            TextureFrom = (GenericValueModel<TextureFrom>)TextureFrom.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (SelectRandomTextureModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            TextureFrom.FromModel(m.TextureFrom);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.ClearAll();
        }
    }
}
