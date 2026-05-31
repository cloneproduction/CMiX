// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : ObservableObject, IPrefab, ITextureFilter
    {
        public Pixelate(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<BlendModeEnum> blendMode,
                        Vector2 factor)
        {
            PrefabService = prefabService;
            Control = control;
            BlendMode = blendMode;
            Factor = factor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Vector2 Factor { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new PixelateModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Factor = (Vector2Model)Factor.ToModel(),
            BlendMode = (GenericValueModel<BlendModeEnum>)BlendMode.ToModel(),

        };

        public void FromModel(IControlModel model)
        {
            var m = (PixelateModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Control.FromModel(m.Control);
            Factor.FromModel(m.Factor);
            BlendMode.FromModel(m.BlendMode);
        }
    }
}
