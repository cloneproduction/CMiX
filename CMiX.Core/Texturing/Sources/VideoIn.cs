// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoIn : ObservableRecipient, ITextureSource, IDisposable
    {
        public VideoIn(PrefabService prefabService, 
                       PrefabManager textureModifierManager,
                       GenericValue<int> sizeX, 
                       GenericValue<int> sizeZ)
        {
            TextureModifierManager = textureModifierManager;
            PrefabService = prefabService;
            SizeX = sizeX;
            SizeY = sizeZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> SizeX { get; set; }
        public GenericValue<int> SizeY { get; set; }
        public PrefabService PrefabService { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabManager TextureModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new VideoInModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            SizeX = (GenericValueModel<int>)SizeX.ToModel(),
            SizeY = (GenericValueModel<int>)SizeY.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (VideoInModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SizeX.FromModel(m.SizeX);
            SizeY.FromModel(m.SizeY);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
        }

        // The filter modifiers are reachable through this manager alone, so a texture torn down
        // without disposing it leaves their repository and their deleter registrations behind.
        public void Dispose()
        {
            TextureModifierManager.Dispose();
        }
    }
}
