// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : TextureFilterBase
    {
        public Pixelate(PrefabService prefabService,
                        ModulatableValue<float> factorX,
                        ModulatableValue<float> factorY,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { factorX, factorY };
        }

        public ModulatableValue<float> X => Bindables[0];
        public ModulatableValue<float> Y => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new PixelateModel
            {
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (PixelateModel)model;
            LoadBaseModel(m);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
        }
    }
}
