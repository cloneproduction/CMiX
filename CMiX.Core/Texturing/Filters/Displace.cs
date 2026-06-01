// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Displace : ObservableObject, IPrefab, ITextureFilter
    {
        public Displace(PrefabService prefabService, 
                        PrefabManager textureSelector,
                        Vector2 offset,
                        Vector2 offsetScale, 
                        GenericValue<float> control,
                        Blend blend)
        {
            PrefabService = prefabService;
            Offset = offset;
            OffsetScale = offsetScale;
            TextureSelector = textureSelector;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManager TextureSelector { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 OffsetScale { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new DisplaceModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TextureSelector = (PrefabManagerModel)TextureSelector.ToModel(),
            Offset = (Vector2Model)Offset.ToModel(),
            OffsetScale = (Vector2Model)OffsetScale.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DisplaceModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Offset.FromModel(m.Offset);
            OffsetScale.FromModel(m.OffsetScale);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);

            LoadManager(TextureSelector, m.TextureSelector);
        }
    }
}
