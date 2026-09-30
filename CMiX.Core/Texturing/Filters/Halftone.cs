// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Halftone : TextureFilterBase
    {
        public Halftone(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<HalftoneMode> mode,
                        GenericValue<float> numberOfTiles,
                        GenericValue<float> dotSize,
                        GenericValue<float> softness,
                        GenericValue<float> brightness,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Mode = mode;
            NumberOfTiles = numberOfTiles;
            DotSize = dotSize;
            Softness = softness;
            Brightness = brightness;
        }

        public GenericValue<HalftoneMode> Mode { get; set; }
        public GenericValue<float> NumberOfTiles { get; set; }
        public GenericValue<float> DotSize { get; set; }
        public GenericValue<float> Softness { get; set; }
        public GenericValue<float> Brightness { get; set; }

        public override IControlModel ToModel()
        {
            var model = new HalftoneModel
            {
                Mode = (GenericValueModel<HalftoneMode>)Mode.ToModel(),
                NumberOfTiles = (GenericValueModel<float>)NumberOfTiles.ToModel(),
                DotSize = (GenericValueModel<float>)DotSize.ToModel(),
                Softness = (GenericValueModel<float>)Softness.ToModel(),
                Brightness = (GenericValueModel<float>)Brightness.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HalftoneModel)model;
            LoadBaseModel(m);
            Mode.FromModel(m.Mode);
            NumberOfTiles.FromModel(m.NumberOfTiles);
            DotSize.FromModel(m.DotSize);
            Softness.FromModel(m.Softness);
            Brightness.FromModel(m.Brightness);
        }
    }
}
