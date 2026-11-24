// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Liquish : TextureSourceBase, ITextureSource
    {
        public Liquish(PrefabService prefabService,
                       PrefabManager textureModifierManager,
                       ModulatorHost modulators,
                       Integer2 resolution,
                       GenericValue<bool> useCompositionResolution,
                       ModulatableValue<float> time,
                       ModulatableVector2 scale,
                       GenericValue<string> colorA,
                       GenericValue<string> colorB,
                       GenericValue<string> colorC)
            : base(prefabService, textureModifierManager, modulators, useCompositionResolution)
        {
            Resolution = resolution;
            Time = time;
            Scale = scale;
            ColorA = colorA;
            ColorB = colorB;
            ColorC = colorC;

            Modulators.Add(Time, Scale.X, Scale.Y);
        }

        public Integer2 Resolution { get; set; }
        public ModulatableValue<float> Time { get; set; }
        public ModulatableVector2 Scale { get; set; }
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }
        public GenericValue<string> ColorC { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LiquishModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Time = (ModulatableValueModel<float>)Time.ToModel(),
                Scale = (ModulatableVector2Model)Scale.ToModel(),
                ColorA = (GenericValueModel<string>)ColorA.ToModel(),
                ColorB = (GenericValueModel<string>)ColorB.ToModel(),
                ColorC = (GenericValueModel<string>)ColorC.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LiquishModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Time.FromModel(m.Time);
            Scale.FromModel(m.Scale);
            ColorA.FromModel(m.ColorA);
            ColorB.FromModel(m.ColorB);
            ColorC.FromModel(m.ColorC);
        }
    }
}
