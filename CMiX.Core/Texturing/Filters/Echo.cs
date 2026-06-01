// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : ObservableObject, IPrefab, ITextureFilter
    {
        public Echo(PrefabService prefabService,
                    GenericValue<float> factor,
                    GenericValue<float> control,
                    Blend blend)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            Factor = factor;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Factor { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new EchoModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Factor = (GenericValueModel<float>)Factor.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (EchoModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Factor.FromModel(m.Factor);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
