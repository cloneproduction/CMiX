// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Views.Texturing.Filter
{
    public partial class Halftone : ObservableObject, IPrefab, ITextureFilter
    {
        public Halftone(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<HalftoneMode> mode,
                        GenericValue<float> numberOfTiles,
                        GenericValue<float> dotSize,
                        GenericValue<float> softness,
                        GenericValue<float> brightness,
                        Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            Mode = mode;
            NumberOfTiles = numberOfTiles;
            DotSize = dotSize;
            Softness = softness;
            Brightness = brightness;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<HalftoneMode> Mode { get; set; }
        public GenericValue<float> NumberOfTiles { get; set; }
        public GenericValue<float> DotSize { get; set; }
        public GenericValue<float> Softness { get; set; }
        public GenericValue<float> Brightness { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new HalftoneModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Mode = (GenericValueModel<HalftoneMode>)Mode.ToModel(),
            NumberOfTiles = (GenericValueModel<float>)NumberOfTiles.ToModel(),
            DotSize = (GenericValueModel<float>)DotSize.ToModel(),
            Softness = (GenericValueModel<float>)Softness.ToModel(),
            Brightness = (GenericValueModel<float>)Brightness.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (HalftoneModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Control.FromModel(m.Control);
            Mode.FromModel(m.Mode);
            NumberOfTiles.FromModel(m.NumberOfTiles);
            DotSize.FromModel(m.DotSize);
            Softness.FromModel(m.Softness);
            Brightness.FromModel(m.Brightness);
            Blend.FromModel(m.Blend);
        }
    }
}
