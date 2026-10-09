// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    public class FFTModulator : ModulatorBase
    {
        public FFTModulator(PrefabService prefabService,
                            GenericValue<float> fft,
                            GenericValue<float> bass,
                            GenericValue<float> lowerMid,
                            GenericValue<float> higherMid,
                            GenericValue<float> high)
            : base(prefabService)
        {
            FFT = fft;
            Bass = bass;
            LowerMid = lowerMid;
            HigherMid = higherMid;
            High = high;
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
        {
            new ModulatorOutput<float>("FFT"),
            new ModulatorOutput<float>("Bass"),
            new ModulatorOutput<float>("LowerMid"),
            new ModulatorOutput<float>("HigherMid"),
            new ModulatorOutput<float>("High"),
        };

        public GenericValue<float> FFT { get; set; }
        public GenericValue<float> Bass { get; set; }
        public GenericValue<float> LowerMid { get; set; }
        public GenericValue<float> HigherMid { get; set; }
        public GenericValue<float> High { get; set; }


        public override IControlModel ToModel() => new FFTModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FFT = (GenericValueModel<float>)FFT.ToModel(),
            Bass = (GenericValueModel<float>)Bass.ToModel(),
            LowerMid = (GenericValueModel<float>)LowerMid.ToModel(),
            HigherMid = (GenericValueModel<float>)HigherMid.ToModel(),
            High = (GenericValueModel<float>)High.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (FFTModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            FFT.FromModel(m.FFT);
            Bass.FromModel(m.Bass);
            LowerMid.FromModel(m.LowerMid);
            HigherMid.FromModel(m.HigherMid);
            High.FromModel(m.High);
        }
    }
}
