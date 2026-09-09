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
                            GenericValue<float> bass)
        {
            PrefabService = prefabService;
            FFT = fft;
            Bass = bass;
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
            new ModulatorOutput<float>("Bass")
        };

        public GenericValue<float> FFT { get; set; }
        public GenericValue<float> Bass { get; set; }

        public IControlModel ToModel() => new FFTModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FFT = (GenericValueModel<float>)FFT.ToModel(),
            Bass = (GenericValueModel<float>)Bass.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (FFTModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            FFT.FromModel(m.FFT);
            Bass.FromModel(m.Bass);
        }
    }
}
