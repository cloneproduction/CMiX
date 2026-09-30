// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Halftone : TextureFilterBase
    {
        public Halftone(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<HalftoneMode> mode,
                        ModulatableValue<float> numberOfTiles,
                        ModulatableValue<float> dotSize,
                        ModulatableValue<float> softness,
                        ModulatableValue<float> brightness,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Mode = mode;
            Bindables = new List<ModulatableValue<float>> { numberOfTiles, dotSize, softness, brightness };

            numberOfTiles.Label = "Tile Count";
            dotSize.Label = "Dot Size";
            softness.Label = "Softness";
            brightness.Label = "Brightness";
            numberOfTiles.SetDefault(48.0f);
            dotSize.SetDefault(0.01f);
            softness.SetDefault(1.35f);
            brightness.SetDefault(1.0f);
        }

        public GenericValue<HalftoneMode> Mode { get; set; }
        public ModulatableValue<float> NumberOfTiles => Bindables[0];
        public ModulatableValue<float> DotSize => Bindables[1];
        public ModulatableValue<float> Softness => Bindables[2];
        public ModulatableValue<float> Brightness => Bindables[3];

        public override IControlModel ToModel()
        {
            var model = new HalftoneModel
            {
                Mode = (GenericValueModel<HalftoneMode>)Mode.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HalftoneModel)model;
            LoadBaseModel(m);
            Mode.FromModel(m.Mode);
        }
    }
}
