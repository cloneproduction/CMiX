// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : ObservableObject, IPrefab, ITextureFilter
    {
        public Feedback(PrefabService prefabService,
                        GenericValue<float> factor,
                        GenericValue<float> control,
                        Blend blend)
        {
            PrefabService = prefabService;
            Factor = factor;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Factor { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new FeedbackModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Factor = (GenericValueModel<float>)Factor.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (FeedbackModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Factor.FromModel(m.Factor);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
