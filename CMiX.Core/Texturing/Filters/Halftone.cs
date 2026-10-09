// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
                        GenericValue<HalftoneMode> mode,
                        ModulatableValue<float> numberOfTiles,
                        ModulatableValue<float> dotSize,
                        ModulatableValue<float> softness,
                        ModulatableValue<float> brightness,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Mode = mode;
            Bindables = new List<ModulatableValue<float>> { numberOfTiles, dotSize, softness, brightness };
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
                Mode = (GenericValueModel<HalftoneMode>)Mode.ToModel(),
                NumberOfTiles = (ModulatableValueModel<float>)NumberOfTiles.ToModel(),
                DotSize = (ModulatableValueModel<float>)DotSize.ToModel(),
                Softness = (ModulatableValueModel<float>)Softness.ToModel(),
                Brightness = (ModulatableValueModel<float>)Brightness.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HalftoneModel)model;
            LoadBaseModel(m);
            NumberOfTiles.FromModel(m.NumberOfTiles);
            DotSize.FromModel(m.DotSize);
            Softness.FromModel(m.Softness);
            Brightness.FromModel(m.Brightness);
            Mode.FromModel(m.Mode);
        }
    }
}
