// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Modulation.Modulators
{
    public partial class FFTModulator : ObservableObject, IModulator
    {
        public FFTModulator(PrefabService prefabService,
                            GenericValue<float> fft,
                            GenericValue<float> bass,
                            GenericValue<float> lowerMid,
                            GenericValue<float> higherMid,
                            GenericValue<float> high)
        {
            PrefabService = prefabService;
            FFT = fft;
            Bass = bass;
            LowerMid = lowerMid;
            HigherMid = higherMid;
            High = high;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isHovered;

        [ObservableProperty]
        private bool isExpanded = true;

        public IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
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


        public IControlModel ToModel() => new FFTModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FFT = (GenericValueModel<float>)FFT.ToModel(),
            Bass = (GenericValueModel<float>)Bass.ToModel(),
            LowerMid = (GenericValueModel<float>)LowerMid.ToModel(),
            HigherMid = (GenericValueModel<float>)HigherMid.ToModel(),
            High = (GenericValueModel<float>)High.ToModel()
        };

        public void FromModel(IControlModel model)
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
