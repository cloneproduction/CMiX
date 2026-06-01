// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Dither : ObservableObject, IPrefab, ITextureFilter
    {
        public Dither(PrefabService prefabService,
                      GenericValue<float> control,
                      GenericValue<float> threshold,
                      Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            Threshold = threshold;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Threshold { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new DitherModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Threshold = (GenericValueModel<float>)Threshold.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (DitherModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Threshold.FromModel(m.Threshold);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
