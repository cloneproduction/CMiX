// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kuwahara : ObservableObject, IPrefab, ITextureFilter
    {
        public Kuwahara(PrefabService prefabService,
                        GenericValue<float> radius,
                        GenericValue<KuwaharaType> type,
                        GenericValue<float> control)
        {
            PrefabService = prefabService;
            Radius = radius;
            Control = control;
            Type = type;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<KuwaharaType> Type { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new KuwaharaModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Radius = (GenericValueModel<float>)Radius.ToModel(),
            Type = (GenericValueModel<KuwaharaType>)Type.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (KuwaharaModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Radius.FromModel(m.Radius);
            Type.FromModel(m.Type);
            Control.FromModel(m.Control);
        }
    }
}
