// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LEDPanel : ObservableObject, IPrefab, ITextureFilter
    {
        public LEDPanel(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<float> pixelSize,
                        GenericValue<float> maskStagger,
                        GenericValue<float> maskBorder,
                        GenericValue<float> maskIntensity,
                        Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            PixelSize = pixelSize;
            MaskStagger = maskStagger;
            MaskBorder = maskBorder;
            MaskIntensity = maskIntensity;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<float> PixelSize { get; set; }
        public GenericValue<float> MaskStagger { get; set; } 
        public GenericValue<float> MaskBorder { get; set; }
        public GenericValue<float> MaskIntensity { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new LEDPanelModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            PixelSize = (GenericValueModel<float>)PixelSize.ToModel(),
            MaskStagger = (GenericValueModel<float>)MaskStagger.ToModel(),
            MaskBorder = (GenericValueModel<float>)MaskBorder.ToModel(),
            MaskIntensity = (GenericValueModel<float>)MaskIntensity.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LEDPanelModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Control.FromModel(m.Control);
            PixelSize.FromModel(m.PixelSize);
            MaskStagger.FromModel(m.MaskStagger);
            MaskBorder.FromModel(m.MaskBorder);
            MaskIntensity.FromModel(m.MaskIntensity);
            Blend.FromModel(m.Blend);
        }
    }
}
